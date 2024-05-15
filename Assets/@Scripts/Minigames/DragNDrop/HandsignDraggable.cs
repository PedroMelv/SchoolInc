using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandsignDraggable : DraggableUI
{
    [SerializeField] private LayerMask dropLayer;

    private Collider2D[] colliders = new Collider2D[1];

    private AlphabetObject alphabetObject;

    protected override void Awake()
    {
        base.Awake();
        alphabetObject = GetComponent<AlphabetObject>();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if(alphabetObject.attachedOn.Value != null) alphabetObject.attachedOn.Value.attachedOn.Set(null);
        alphabetObject.attachedOn.Set(null);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i] = null;
        }
        
        Physics2D.OverlapBoxNonAlloc(Input.mousePosition, rectTransform.sizeDelta / 10, 0f, colliders, dropLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null) continue;

            if (colliders[i].gameObject.TryGetComponent(out AlphabetObject other))
            {
                if(other.attachedOn.Value != null) continue;
                other.attachedOn.Set(alphabetObject);
                alphabetObject.attachedOn.Set(other);

                rectTransform.SetAsFirstSibling();
                rectTransform.localPosition = colliders[i].transform.localPosition;
                break;
                
            }
        }

    }

    private void OnDrawGizmos()
    {
        if(rectTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(dragPosition, rectTransform.sizeDelta / 10);
        }
        
    }
}
