using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private bool bringToFrontOnBeginDrag;
    private Vector2 offset;
    protected Vector2 dragPosition;

    protected bool isOnCanvas = false;

    protected RectTransform rectTransform;
    protected RectTransform parentRectTransform;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
        if (parentRectTransform.TryGetComponent(out Canvas canvas))
        {
            isOnCanvas = true;
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (bringToFrontOnBeginDrag) rectTransform.SetAsLastSibling();
        offset = (Vector2)transform.position - eventData.position;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        dragPosition = eventData.position;
        transform.position = eventData.position + offset;

        ClampDraggable();
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        offset = Vector3.zero;
    }

    protected virtual void ClampDraggable()
    {
        Vector3 pos = rectTransform.localPosition;

        float halfWidth = rectTransform.rect.width / 2;
        float halfHeight = rectTransform.rect.height / 2;

        float minX = -parentRectTransform.rect.width / 2 + halfHeight;
        float maxX = parentRectTransform.rect.width / 2 - halfHeight;
        float minY = -parentRectTransform.rect.height / 2 + halfHeight;
        float maxY = parentRectTransform.rect.height / 2 - halfHeight;

        if (isOnCanvas)
        {
            maxY -= AdsSystem.Instance.ShowingBanner ? AdsSystem.Instance.BannerHeight : 0;
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rectTransform.localPosition = pos;
    }
}
