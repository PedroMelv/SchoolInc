using DG.Tweening;
using System;
using UnityEngine;

public class SlideUI : MonoBehaviour
{
    public enum PositionStyle
    {
        RELATIVE_TO_CANVAS,
        ADITIVE,
        ADITIVE_RELATIVE_TO_CANVAS,
        NONE
    }

    [SerializeField] private bool slideOnStart = true;
    [SerializeField, Tooltip("Define a posição inicial no método Start()")] private bool setStartOnStart = true;
    public PositionStyle targetPositionStyle = PositionStyle.ADITIVE;
    [SerializeField] private Vector3 targetPosition;
    public PositionStyle startPositionStyle = PositionStyle.NONE;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 targetScale = Vector3.one;
    [SerializeField] private Vector3 startScale = Vector3.one;
    [SerializeField] private float slideDuration;
    [SerializeField] private Ease slideEase;

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private RectTransform rectTrans;

    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }

    private void Awake()
    {
        GetComponents();
    }

    private void Start()
    {
        if (setStartOnStart)
        {
            startPosition = rectTrans.localPosition;
            startScale = transform.localScale;
        }
        if (slideOnStart) ExecuteSlide();
    }

    private void GetComponents()
    {
        mainCanvas = GetComponentInParent<Canvas>();
        rectTrans = GetComponent<RectTransform>();
    }

    public void SetTargetPositionToCurrent()
    {
        targetPosition = TransformPosition(rectTrans.localPosition, targetPositionStyle);
    }
    public void SetStartPositionToCurrent()
    {
        startPosition = TransformPosition(rectTrans.localPosition, startPositionStyle);
    }

    public void ExecuteSlide()
    {
        ExecuteSlide(0, false);
    }

    public void ExecuteSlide(float delay = 0, bool reseting = false)
    {
        ExecuteSlide(null, delay, reseting);
    }
    public void ExecuteSlide(Action onComplete, float delay = 0, bool reseting = false)
    {
        if (reseting)
        {
            ResetPositionAndScale();
        }
        transform.DOMove(TransformPosition(TargetPosition, targetPositionStyle), slideDuration).SetEase(slideEase).onComplete += ()=> onComplete?.Invoke();
        transform.DOScale(targetScale, slideDuration).SetEase(slideEase).SetDelay(delay);
    }
    public void ResetPositionAndScale()
    {
        transform.DOKill(true);
        transform.localPosition = startPosition;
        transform.localScale = startScale;
    }
    public void ResetSlide(float delay = 0)
    {
        ResetPositionAndScale();
        ExecuteSlide(delay);
    }

    public void ExecuteReversalSlide()
    {
        ExecuteReversalSlide(0, false);
    }

    public void ExecuteReversalSlide(float delay = 0, bool reseting = false)
    {
        ExecuteReversalSlide(null, delay, reseting);
    }

    public void ExecuteReversalSlide(Action onComplete, float delay = 0, bool reseting = false)
    {
        if (reseting)
        {
            ResetPositionAndScale();
        }
        transform.DOMove(TransformPosition(startPosition, startPositionStyle), slideDuration).SetDelay(delay).SetEase(slideEase).onComplete += ()=> onComplete?.Invoke();
        transform.DOScale(targetScale, slideDuration).SetEase(slideEase).SetDelay(delay);
    }

    public void SlideToPosition(Vector3 position, PositionStyle positionStyle, float duration = -1, float delay = 0)
    {
        SlideToPosition(position, positionStyle, Vector3.one, duration, delay);
    }
    public void SlideToPosition(Vector3 position, PositionStyle positionStyle, Vector3 scale, float duration = -1, float delay = 0)
    {
        transform.DOKill(true);
        if (duration == -1) duration = slideDuration;
        transform.DOMove(TransformPosition(position, positionStyle), duration).SetDelay(delay);
        transform.DOScale(scale, duration).SetDelay(delay);
    }
    public void SlideFromPosition(Vector3 position, float duration = -1, float delay = 0)
    {
        SlideFromPosition(position, Vector3.one, duration, delay);
    }
    public void SlideFromPosition(Vector3 position, Vector3 scale, float duration = -1, float delay = 0)
    {
        transform.DOKill(true);
        transform.position = position * mainCanvas.transform.localScale.x;
        transform.localScale = scale;
        transform.DOMove(TransformPosition(TargetPosition, targetPositionStyle), duration).SetDelay(delay);
        transform.DOScale(targetScale, duration).SetDelay(delay);
    }
    public void SlideFromToPosition(Vector3 from, PositionStyle fromPositionStyle, Vector3 to, PositionStyle toPositionStyle, float duration = -1, float delay = 0)
    {
        SlideFromToPosition(from, Vector3.one, fromPositionStyle, to, Vector3.one, toPositionStyle, duration, delay);
    }
    public void SlideFromToPosition(Vector3 from, Vector3 fromScale, PositionStyle fromPositionStyle, Vector3 to, Vector3 toScale, PositionStyle toPositionStyle, float duration = -1, float delay = 0)
    {
        transform.DOKill(true);
        transform.position = TransformPosition(from, fromPositionStyle);
        transform.localScale = fromScale;
        transform.DOMove(TransformPosition(to, toPositionStyle), duration).SetDelay(delay);
        transform.DOScale(toScale, duration).SetDelay(delay);
    }

    public Vector3 TransformPosition(Vector3 pos, PositionStyle positionStyle)
    {
        Vector3 position = pos * mainCanvas.transform.localScale.y;

        switch (positionStyle)
        {
            case PositionStyle.NONE:
                position = mainCanvas.transform.position + pos * mainCanvas.transform.localScale.y;
                break;
            case PositionStyle.ADITIVE:
                position = transform.position + position;
                break;
            case PositionStyle.RELATIVE_TO_CANVAS:
                position = mainCanvas.transform.TransformPoint(position);
                break;
            case PositionStyle.ADITIVE_RELATIVE_TO_CANVAS:
                position = transform.position + mainCanvas.transform.TransformPoint(position);
                break;
        }

        return position;
    }

    public Vector3 DeTransformPosition(Vector3 pos, PositionStyle positionStyle)
    {
        Vector3 position = pos;

        switch (positionStyle)
        {
            case PositionStyle.NONE:
                position = (pos / mainCanvas.transform.localScale.y) - mainCanvas.transform.position;
                break;
            case PositionStyle.ADITIVE:
                position = (position - transform.position) / mainCanvas.transform.localScale.y;
                break;
            case PositionStyle.RELATIVE_TO_CANVAS:
                position = mainCanvas.transform.InverseTransformPoint(position) / mainCanvas.transform.localScale.y;
                break;
            case PositionStyle.ADITIVE_RELATIVE_TO_CANVAS:
                position = mainCanvas.transform.InverseTransformPoint(position - transform.position) / mainCanvas.transform.localScale.y;
                break;
        }

        return position;
    }

    public Vector3 GetCanvasOffset()
    {
        GetComponents();

        return mainCanvas.transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (rectTrans == null || mainCanvas == null)
        {
            GetComponents();
            return;
        }

        Gizmos.DrawWireCube(TransformPosition(targetPosition, targetPositionStyle), rectTrans.rect.size * mainCanvas.transform.localScale.x);
  
        Gizmos.DrawSphere(TransformPosition(targetPosition, targetPositionStyle), 1f);
        Gizmos.DrawLine(TransformPosition(targetPosition, targetPositionStyle), transform.position);
    }
}
