using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MinigameHandler : StaticInstance<MinigameHandler>
{
    [SerializeField] protected GameCurrency gameCurrency;
    [Space]
    [SerializeField] protected RectTransform minigamePanel;
    [Space]
    [SerializeField] protected GameObject minigameDefeatUI;
    [SerializeField] protected Button minigameDefeatCloseButton;
    [SerializeField] protected Button minigameTryAgainButton;
    [Space]
    [SerializeField] protected GameObject minigameVictoryUI;
    [SerializeField] protected Button minigameVictoryCloseButton;
    [SerializeField] protected Button minigameDoubleButton;
    [SerializeField] protected TextMeshProUGUI victoryMoneyText;
    [Space]
    [SerializeField] protected Image timerImage;

    [SerializeField] protected float minigameDuration = 15;
    protected float minigameTimer = 0;

    protected UnityEvent onDefeat;
    protected UnityEvent onVictory;

    protected UnityAction onDefeatCallback;
    protected UnityAction onVictoryCallback;

    protected Coroutine resultCoroutine;

    protected double monetaryPrize;

    protected bool closeMinigameInput = false;
    protected bool playingMinigame = false;

    public virtual void InitializeMinigame(UnityAction victoryCallback, UnityAction defeatCallback, double monetaryPrize = 0)
    {
        minigamePanel.gameObject.SetActive(true);

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

    public virtual void InitializeMinigame(double monetaryPrize = 0)
    {
        InitializeMinigame(onVictoryCallback, onDefeatCallback, monetaryPrize);
    }

    protected virtual void Defeat()
    {
        ShowResult(false);

        playingMinigame = false;

    }
    protected virtual void Victory()
    {
        ShowResult(true);

        playingMinigame = false;
    }

    protected virtual void ShowResult(bool result)
    {
        resultCoroutine = StartCoroutine(result ? EShowVictory() : EShowDefeat());
    }

    protected virtual IEnumerator EShowVictory()
    {
        yield break;
    }

    protected virtual IEnumerator EShowDefeat()
    {
        yield break;
    }

    public virtual void DoublePrizeButton()
    {
        AdsSystem.PlayRewarded(() =>
        {
            //Completed
            monetaryPrize *= 2;
            BigInteger winPrize = new BigInteger(monetaryPrize);
        },
        null
        );
    }

    public virtual void TryAgainButton()
    {
        AdsSystem.PlayRewarded(() =>
        {
            if (resultCoroutine != null) StopCoroutine(resultCoroutine);
            InitializeMinigame(monetaryPrize);
        },
        null
        );
    }

    public virtual void CloseMinigame(bool showAd)
    {
        if (showAd) AdsSystem.PlayIntersistial();
        closeMinigameInput = true;
    }
}
