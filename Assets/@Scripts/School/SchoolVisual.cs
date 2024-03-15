using DG.Tweening;
using TMPro;
using UnityEngine;

public class SchoolVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro costTextPrefab;
    [SerializeField] private GameObject progressionSliderPrefab;

    [Space]
    [SerializeField] private Vector3 cameraOffsetPosition;
    public Vector3 CameraOffsetPosition { get => cameraOffsetPosition; set => cameraOffsetPosition = value; }
    [Space]
    private TextMeshPro costText;
    [Space]
    private GameObject progressionSlider;
    private Transform progressionSlider_fill;

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

        Vector3 lastPos = boxTop + distanceTop;

        progressionSlider = Instantiate(progressionSliderPrefab, lastPos, Quaternion.identity);
        progressionSlider_fill = progressionSlider.transform.Find("Fill");
        costText = Instantiate(costTextPrefab, lastPos, Quaternion.identity);

        SetProgressionSliderForced(0f);
        SetCostText();


        SetCostText(!data.isUnlocked);
        SetProgressionSlider(data.isUnlocked);


    }

    public void SetCostText(bool visible)
    {
        costText.gameObject.SetActive(visible);
        SetCostText();
    }
    public void SetCostText()
    {
        costText.text = "Buy Cost: " + MoneyUtils.MoneyString(data.initialCost);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cameraOffsetPosition + transform.position, 1f);
        Gizmos.color = Color.yellow;

        Vector3 distanceTop = new Vector3(0f, 7f, 0f);
        float scale = transform.localScale.y;
        Vector3 boxTop = transform.position + Vector3.up * 10 * scale;

        Gizmos.DrawCube(boxTop + distanceTop, new Vector3(1f,1f,1f));
        Gizmos.color = Color.green;
        Gizmos.DrawCube(boxTop, new Vector3(1f,1f,1f));
    }
}
