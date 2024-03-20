using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TappableObject : MonoBehaviour
{
    [SerializeField] private float tapShrinkScale = .95f;
    [SerializeField] private float shrinkDuration = 0.05f;
    private Vector3 defaultScale;


    public Func<bool> canTap;

    public Action onTap;
    public Action onTapTimed;
    
    public Action onComplete;

    protected AutomaticTapper automatic;
    public AutomaticTapper Automatic { get => automatic; }


    protected virtual void Awake()
    {
        automatic = GetComponent<AutomaticTapper>();
    }

    protected virtual void Start()
    {
        onComplete += ShrinkEffect;
        defaultScale = transform.localScale;
    }

    public virtual void Tap(bool useShrink = true)
    {
        onTap?.Invoke();
    }
    public virtual void TapWithTime()
    {
        onTapTimed?.Invoke();
    }


    private void ShrinkEffect()
    {
        gameObject.transform.DOKill(false);
        gameObject.transform.localScale = defaultScale;

        gameObject.transform.DOScale(defaultScale * tapShrinkScale, shrinkDuration).onComplete 
            += ()=> gameObject.transform.DOScale(defaultScale, shrinkDuration);
        
    }

}
