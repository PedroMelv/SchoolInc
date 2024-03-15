using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticTapper : MonoBehaviour
{
    protected TappableObject tappable;

    protected Coroutine tapper;

    protected bool isInfinity = false;
    
    public bool IsInfinity { get { return isInfinity; } }

    protected virtual void Awake()
    {
        tappable = GetComponent<TappableObject>();
    }

    private void Start()
    {
        tappable.canTap += ()=>
        {
            if (tapper == null)
            {
                StartTapper();
                return false;
            }
            return true;
        };
        tappable.onComplete += StopTapper;
    }

    public virtual void StartTapper()
    {
        if(tapper != null)
        {
            return;
        }

        tapper = StartCoroutine(ETapper());
    }
    public virtual void StopTapper()
    {
        if (isInfinity) return;

        StopCoroutine(tapper);
        tapper = null;
    }

    public virtual void SetInfinity(bool isInfinity)
    {
        this.isInfinity = isInfinity;

    }

    protected virtual IEnumerator ETapper()
    {
        while(true)
        {
            tappable.TapWithTime();
            yield return new WaitForEndOfFrame();
        }
    }
}
