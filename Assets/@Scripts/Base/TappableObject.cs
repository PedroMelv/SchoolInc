using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TappableObject : MonoBehaviour
{
    [SerializeField] private Vector3 tapShrinkScale = new Vector3(.95f,.95f,.95f);
    [SerializeField] private float shrinkDuration = 0.05f;

    public Func<bool> canTap;

    public Action onTap;
    public Action onTapTimed;
    
    public Action onComplete;

    protected virtual void Start()
    { }

    public virtual void Tap(bool useShrink = true)
    {
        
        if(useShrink) ShrinkEffect();
        
        onTap?.Invoke();
    }
    public virtual void TapWithTime()
    {
        onTapTimed?.Invoke();
    }


    private void ShrinkEffect()
    {
        gameObject.transform.DOKill(false);
        gameObject.transform.localScale = Vector3.one;

        gameObject.transform.DOScale(tapShrinkScale, shrinkDuration).onComplete 
            += ()=> gameObject.transform.DOScale(Vector3.one, shrinkDuration);
        
    }

}
