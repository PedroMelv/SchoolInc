using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DragNDropMinigameHandler : StaticInstance<DragNDropMinigameHandler>
{
    [SerializeField] private GameCurrency gameCurrency;
    [Space]
    [SerializeField] private RectTransform minigamePanel;
    [Space]
    [SerializeField] private AlphabetObject iconPrefab;
    [SerializeField] private AlphabetObject dropPrefab;
    [Space]
    [SerializeField] private Transform iconPlacement;
    [SerializeField] private Transform dropPlacement;
    [Space]
    [SerializeField] private GameObject minigameDefeatUI;
    [SerializeField] private Button minigameDefeatCloseButton;
    [SerializeField] private Button minigameTryAgainButton;
    [Space]
    [SerializeField] private GameObject minigameVictoryUI;
    [SerializeField] private Button minigameVictoryCloseButton;
    [SerializeField] private Button minigameDoubleButton;
    [SerializeField] private TextMeshProUGUI victoryMoneyText;
    [Space]
    [SerializeField] private float minigameDuration = 15;
    [SerializeField] private Image timerImage;

    private bool closeMinigameInput = false;
    private bool playingMinigame = false;
    private float minigameTimer = 0;

    private char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private List<char> letters = new List<char>();

    private List<AlphabetObject> allObjects = new List<AlphabetObject>();

    private UnityEvent onDefeat;
    private UnityEvent onVictory;

    private UnityAction onDefeatCallback;
    private UnityAction onVictoryCallback;

    private Coroutine resultCoroutine;

    private double monetaryPrize;

    private void Start()
    {
        //InitializeMinigame(Random.Range(1000, 100000));
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

    public void InitializeMinigame(UnityAction victoryCallback, UnityAction defeatCallback, double monetaryPrize = 0)
    {
        ClearObjects();

        minigamePanel.gameObject.SetActive(true);

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

        if (onVictory == null) onVictory = new UnityEvent();
        else onVictory.RemoveAllListeners();

        if (onDefeat == null) onDefeat = new UnityEvent();
        else onDefeat.RemoveAllListeners();

        if (victoryCallback != null) onVictory.AddListener(victoryCallback);
        if (defeatCallback != null) onDefeat.AddListener(defeatCallback);

        onVictoryCallback = victoryCallback;
        onDefeatCallback = defeatCallback;

        closeMinigameInput = false;
        this.monetaryPrize = monetaryPrize;
        minigameTimer = minigameDuration;
        playingMinigame = true;
    }
    public void InitializeMinigame(double monetaryPrize = 0)
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
    private void Defeat()
    {
        ShowResult(false);
        
        ClearObjects();
        playingMinigame = false;
        
    }
    private void Victory()
    {
        ShowResult(true);
        
        ClearObjects();
        playingMinigame = false;
    }

    private void ShowResult(bool result)
    {
        resultCoroutine = StartCoroutine(result ? EShowVictory() : EShowDefeat());
    }

    private IEnumerator EShowVictory()
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
    }

    private IEnumerator EShowDefeat()
    {
        minigameDefeatUI.SetActive(true);
        onDefeat?.Invoke();

        while (!closeMinigameInput) yield return null;

        minigameDefeatUI.SetActive(false);
        minigamePanel.gameObject.SetActive(false);
    }

    public void DoublePrizeButton()
    {
        minigameDoubleButton.interactable = false;
        minigameVictoryCloseButton.interactable = false;
        AdsSystem.PlayRewarded(() =>
        {
            //Completed
            monetaryPrize *= 2;
            BigInteger winPrize = new BigInteger(monetaryPrize);
            victoryMoneyText.SetText(MoneyUtils.MoneyString(winPrize, "+$"));
            minigameVictoryCloseButton.interactable = true;
        },
        ()=>{
            //Failed
            minigameDoubleButton.interactable = true;
            minigameVictoryCloseButton.interactable = true;
        });
    }

    public void TryAgainButton()
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

    public void CloseMinigame()
    {
        //TODO: Show Intersistial
        closeMinigameInput = true;
    }
}
