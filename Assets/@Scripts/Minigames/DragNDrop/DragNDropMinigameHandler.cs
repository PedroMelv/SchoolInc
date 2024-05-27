using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DragNDropMinigameHandler : MinigameHandler
{
    [SerializeField] private AlphabetObject iconPrefab;
    [SerializeField] private AlphabetObject dropPrefab;
    [Space]
    [SerializeField] private Transform iconPlacement;
    [SerializeField] private Transform dropPlacement;
    

    private char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private List<char> letters = new List<char>();

    private List<AlphabetObject> allObjects = new List<AlphabetObject>();

    private void Start()
    {
        
    }

    private void Update()
    {
        if(!playingMinigame) return;

        minigameTimer -= Time.deltaTime;
        timerImage.fillAmount = minigameTimer / minigameDuration;
        if (minigameTimer <= 0)
        {
            Defeat();
        }
    }

    public override void InitializeMinigame(UnityAction victoryCallback, UnityAction defeatCallback, double monetaryPrize = 0)
    {
        ClearObjects();

        

        letters = new List<char>(alphabet);

        int amountOfLetters = Random.Range(2, 4);

        for (int i = 0; i < amountOfLetters; i++)
        {
            char randomLetter = letters[Random.Range(0, letters.Count)];
            letters.Remove(randomLetter);

            AlphabetObject obj = Instantiate(iconPrefab, iconPlacement);
            obj.Initialize(randomLetter, GetRandomPositionInPanel(obj.RectTransform.rect));
            AlphabetObject tracked = obj;
            obj.attachedOn.AddListener(TrackAttached);

            allObjects.Add(obj);

            obj = Instantiate(dropPrefab, dropPlacement);
            obj.Initialize(randomLetter, GetRandomPositionInPanel(obj.RectTransform.rect));

            allObjects.Add(obj);
        }

        base.InitializeMinigame(victoryCallback, defeatCallback);

        closeMinigameInput = false;
        this.monetaryPrize = monetaryPrize;
        minigameTimer = minigameDuration;
        playingMinigame = true;
    }
    public override void InitializeMinigame(double monetaryPrize = 0)
    {
        InitializeMinigame(onVictoryCallback, onDefeatCallback, monetaryPrize);
    }
    private void TrackAttached(AlphabetObject attachedTo)
    {
        (bool allPlaced, bool allCorrect) = CheckLetters();
        
        if(allPlaced)
        {
            if(allCorrect)
            {
                Victory();
            }
            else
            {
                Defeat();
            }
        }
    }
    private (bool allPlaced, bool allCorrect) CheckLetters()
    {
        bool allCorrect = true;
        bool allPlaced = true;
        for (int i = 0; i < allObjects.Count; i++)
        {
            if (allObjects[i].Draggable == null) continue;

            if (allCorrect && (allObjects[i].attachedOn.Value == null || !allObjects[i].CompareLetters(allObjects[i].attachedOn)))
            {
                allCorrect = false;
            }

            if(allPlaced && allObjects[i].attachedOn.Value == null)
            {
                allPlaced = false;
            }
        }

        return (allPlaced, allCorrect);
    }
    private UnityEngine.Vector3 GetRandomPositionInPanel(Rect target)
    {
        UnityEngine.Vector3 randomPosition = new UnityEngine.Vector3(Random.Range(-minigamePanel.rect.width / 2 + target.width / 2, minigamePanel.rect.width / 2 - target.width / 2), Random.Range(-minigamePanel.rect.height / 2 + target.height / 2, minigamePanel.rect.height / 2 - target.height / 2), 0);

        bool overlapping = true;

        while(overlapping)
        {
            overlapping = false;

            for (int i = 0; i < allObjects.Count; i++)
            {
                
                if (UnityEngine.Vector3.Distance(randomPosition, allObjects[i].transform.localPosition) < target.width)
                {
                    Debug.Log("Distance: " + UnityEngine.Vector3.Distance(randomPosition, allObjects[i].transform.localPosition));
                    overlapping = true;
                    randomPosition = new UnityEngine.Vector3(Random.Range(-minigamePanel.rect.width / 2 + target.width / 2, minigamePanel.rect.width / 2 - target.width / 2), Random.Range(-minigamePanel.rect.height / 2 + target.height / 2, minigamePanel.rect.height / 2 - target.height / 2), 0);
                    break;
                }
            }
        }
            
        return randomPosition;
    }
    private void ClearObjects()
    {
        foreach (AlphabetObject alphabetObject in allObjects)
        {
            Destroy(alphabetObject.gameObject);
        }

        allObjects.Clear();
    }

    public void AddMonetaryPrize(double monetaryPrize)
    {
        this.monetaryPrize += monetaryPrize;
    }
    protected override void Defeat()
    {
        ShowResult(false);
        
        ClearObjects();
        playingMinigame = false;
        
    }
    protected override void Victory()
    {
        ShowResult(true);
        
        ClearObjects();
        playingMinigame = false;
    }

    protected override void ShowResult(bool result)
    {
        resultCoroutine = StartCoroutine(result ? EShowVictory() : EShowDefeat());
    }

    protected override IEnumerator EShowVictory()
    {
        minigameVictoryUI.SetActive(true);
        onVictory?.Invoke();
        
        BigInteger winPrize = new BigInteger(monetaryPrize);
        victoryMoneyText.SetText(MoneyUtils.MoneyString(winPrize, "+$"));

        while (!closeMinigameInput) yield return null;

        gameCurrency.AddCurrency(monetaryPrize);
        monetaryPrize = 0;
        minigameVictoryUI.SetActive(false);
        minigamePanel.gameObject.SetActive(false);

        SaveLoadSystem.Instance.SaveGame();
    }

    protected override IEnumerator EShowDefeat()
    {
        minigameDefeatUI.SetActive(true);
        onDefeat?.Invoke();

        while (!closeMinigameInput) yield return null;

        minigameDefeatUI.SetActive(false);
        minigamePanel.gameObject.SetActive(false);
    }

    public override void DoublePrizeButton()
    {
        minigameDoubleButton.interactable = false;
        minigameVictoryCloseButton.interactable = false;
        AdsSystem.PlayRewarded(() =>
        {
            //Completed
            monetaryPrize *= 2;
            BigInteger winPrize = new BigInteger(monetaryPrize);
            victoryMoneyText.SetText(MoneyUtils.MoneyString(winPrize, "+R$"));
            minigameVictoryCloseButton.interactable = true;
        },
        ()=>{
            //Failed
            minigameDoubleButton.interactable = true;
            minigameVictoryCloseButton.interactable = true;
        });
    }

    public override void TryAgainButton()
    {
        minigameTryAgainButton.interactable = false;
        minigameDefeatCloseButton.interactable = false;
        AdsSystem.PlayRewarded(() =>
        {
            if (resultCoroutine != null) StopCoroutine(resultCoroutine);
            minigameDefeatUI.SetActive(false);
            InitializeMinigame(monetaryPrize);
        },
        ()=>
        {
            minigameDefeatCloseButton.interactable = true;
        });
    }

    public override void CloseMinigame(bool showAd)
    {
        if(showAd)AdsSystem.PlayIntersistial();



        closeMinigameInput = true;
    }
}
