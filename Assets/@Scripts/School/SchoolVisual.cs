using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class SchoolVisual : MonoBehaviour
{
    [HideInInspector]public bool editOffset;

    [SerializeField] private TextMeshPro costTextPrefab;
    [SerializeField] private WorldButton collectMoneyButtonPrefab;
    [SerializeField] private GameObject progressionSliderPrefab;
    [SerializeField] private Vector3 costTextOffset;
    [SerializeField] private Vector3 collectMoneyOffset;
    [SerializeField] private Vector3 progressionSliderOffset;
    [Space]
    [SerializeField] private Vector3 cameraOffsetPosition;
    
    public Vector3 CameraOffsetPosition { get => cameraOffsetPosition; set => cameraOffsetPosition = value; }
    public Vector3 CostTextOffset { get => costTextOffset; set => costTextOffset = value; }
    public Vector3 CollectMoneyOffset { get => collectMoneyOffset; set => collectMoneyOffset = value; }
    public Vector3 ProgressionSliderOffset { get => progressionSliderOffset; set => progressionSliderOffset = value; }

    [Space]
    private TextMeshPro collectMoneyText;
    private TextMeshPro costText;
    [Space]
    private GameObject progressionSlider;
    private Transform progressionSlider_fill;

    private WorldButton collectMoneyButton;

    private SchoolData data;

    private void Awake()
    {
        data = GetComponent<SchoolData>();
        
    }
    private void Start()
    {
        Vector3 distanceTop = new Vector3(0f, 7f, 0f);
        float scale = transform.localScale.y;
        Vector3 boxTop = transform.position + Vector3.up * 10 * scale;

        Vector3 spawnPos = boxTop + distanceTop;
        spawnPos += progressionSliderOffset;
        progressionSlider = Instantiate(progressionSliderPrefab, spawnPos, Quaternion.identity);
        spawnPos -= progressionSliderOffset;

        spawnPos += collectMoneyOffset;
        collectMoneyButton = Instantiate(collectMoneyButtonPrefab, spawnPos, Quaternion.identity);
        collectMoneyText = collectMoneyButton.GetComponentInChildren<TextMeshPro>();
        spawnPos -= collectMoneyOffset;

        progressionSlider_fill = progressionSlider.transform.Find("Fill");
        
        spawnPos += costTextOffset;
        costText = Instantiate(costTextPrefab, spawnPos, Quaternion.identity);
        spawnPos -= costTextOffset;

        SetProgressionSliderForced(0f);
        SetCostText();


        SetCostText(!data.isUnlocked);
        SetProgressionSlider(data.isUnlocked);

        SetCollectButtonActive(false);
    }

    public void SetCostText(bool visible)
    {
        costText.gameObject.SetActive(visible);
        SetCostText();
    }
    public void SetCostText()
    {
        costText.text = "Buy Cost: " + MoneyUtils.MoneyString(data.initialCost, "$");
    }

    public void SetProgressionSlider(bool visible, float amount)
    {
        SetProgressionSlider(visible);
        SetProgressionSlider(amount);
    }
    public void SetProgressionSlider(float amount)
    {
        progressionSlider_fill.DOKill(true);
        progressionSlider_fill.DOScaleX(amount, .1f);
    }
    public void SetProgressionSliderForced(float amount)
    {
        progressionSlider_fill.localScale = new Vector3(amount, 1f,1f);
    }
    public void SetProgressionSlider(bool visible)
    {
        progressionSlider.SetActive(visible);
    }

    public void SetCollectButtonActive(bool isActive, string amount = "")
    {
        collectMoneyText.text = amount;

        collectMoneyButton.gameObject.SetActive(isActive);
    }

    public void AddCollectButtonEvent(Action action)
    {
        collectMoneyButton.OnButtonPressed += action;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cameraOffsetPosition + transform.position, 1f);
        Gizmos.color = Color.yellow;

        Vector3 distanceTop = new Vector3(0f, 7f, 0f);
        float scale = transform.localScale.y;
        Vector3 boxTop = transform.position + Vector3.up * 10 * scale;

        Gizmos.DrawCube(boxTop + distanceTop + collectMoneyOffset, new Vector3(1f,1f,1f));
        Gizmos.DrawCube(boxTop + distanceTop + costTextOffset, new Vector3(1f,1f,1f));
        Gizmos.DrawCube(boxTop + distanceTop + progressionSliderOffset, new Vector3(1f,1f,1f));
        Gizmos.color = Color.green;
        Gizmos.DrawCube(boxTop, new Vector3(1f,1f,1f));
    }
}
