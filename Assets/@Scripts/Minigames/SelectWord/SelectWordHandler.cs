using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class SelectWordHandler : MinigameHandler
{
    private LibraWordSO[] words;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Button[] responseButtons;
    private TextMeshProUGUI[] responseTexts;



    private void Start()
    {
        words = Resources.LoadAll<LibraWordSO>("LibraWords");
        videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
    }

    private void VideoPlayer_prepareCompleted(VideoPlayer source)
    {
        source.Play();
    }

    private void Update()
    {
        if (!playingMinigame) return;

        minigameTimer -= Time.deltaTime;
        timerImage.fillAmount = minigameTimer / minigameDuration;
        if (minigameTimer <= 0)
        {
            Defeat();
        }
    }

    public override void InitializeMinigame(double monetaryPrize = 0)
    {
        InitializeMinigame(null,null,monetaryPrize);
    }

    public override void InitializeMinigame(UnityAction victoryCallback, UnityAction defeatCallback, double monetaryPrize = 0)
    {
        base.InitializeMinigame(victoryCallback, defeatCallback, monetaryPrize);

        for (int i = 0; i < responseButtons.Length; i++)
        {
            responseButtons[i].gameObject.SetActive(true);
        }

        if (responseTexts == null || responseTexts.Length == 0)
        {
            responseTexts = new TextMeshProUGUI[4];
            for (int i = 0; i < responseButtons.Length; i++)
            {
                responseTexts[i] = responseButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        LibraWordSO word = words[Random.Range(0, words.Length)];
        int wordIndex = Random.Range(0, responseButtons.Length);
        responseTexts[wordIndex].text = word.word;

        List<string> incorrects = new List<string>(word.incorrectWords);
        for (int i = 0; i < responseTexts.Length; i++)
        {
            if (i == wordIndex) continue;

            int randomWord = Random.Range(0, incorrects.Count);
            responseTexts[i].text = incorrects[randomWord];
            incorrects.RemoveAt(randomWord);
        }

        minigameTimer = minigameDuration + 1;

        videoPlayer.clip = word.video;
        videoPlayer.Prepare();

        for (int i = 0; i < responseButtons.Length; i++)
        {
            responseButtons[i].onClick.RemoveAllListeners();
            responseButtons[i].onClick.AddListener((i == wordIndex) ? Victory : Defeat);
        }

    }

    protected override void Victory()
    {
        for (int i = 0; i < responseButtons.Length; i++)
        {
            responseButtons[i].gameObject.SetActive(false);
        }
        videoPlayer.Stop();
        base.Victory();
    }

    protected override void Defeat()
    {
        for (int i = 0; i < responseButtons.Length; i++)
        {
            responseButtons[i].gameObject.SetActive(false);
        }
        videoPlayer.Stop();
        base.Defeat();
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
            victoryMoneyText.SetText(MoneyUtils.MoneyString(winPrize, "+$"));
            minigameVictoryCloseButton.interactable = true;
        },
        () => {
            //Failed
            minigameDoubleButton.interactable = true;
            minigameVictoryCloseButton.interactable = true;
        });
    }

    protected override IEnumerator EShowVictory()
    {
        minigameVictoryUI.SetActive(true);
        onVictory?.Invoke();

        BigInteger winPrize = new BigInteger(monetaryPrize);
        victoryMoneyText.SetText(MoneyUtils.MoneyString(winPrize, "+R$"));

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
        () =>
        {
            minigameDefeatCloseButton.interactable = true;
        });
    }

    public override void CloseMinigame(bool showAd)
    {
        if (showAd) AdsSystem.PlayIntersistial();

        

        closeMinigameInput = true;
    }
}
