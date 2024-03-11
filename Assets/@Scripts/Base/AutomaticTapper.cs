using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticTapper : MonoBehaviour
{
    protected TappableObject tappable;
    protected float tick;

    protected Coroutine tapper;


    protected virtual void Awake()
    {
        tappable = GetComponent<TappableObject>();
    }

    public virtual void ChangeTick(float newTick) => tick = newTick;

    public virtual void StartTapper()
    {
        if(tapper != null)
        {
            #if UNITY_EDITOR 
            Debug.LogWarning("Tapper " + gameObject.name + " já estava ativo", this.gameObject) ;
            #endif
            return;
        }

        tapper = StartCoroutine(ETapper());
    }

    public virtual void StopTapper()
    {
        StopCoroutine(ETapper());
        tapper = null;
    }


    protected virtual IEnumerator ETapper()
    {
        while(true)
        {
            tappable.Tap(false);
            yield return new WaitForSeconds(tick);
        }
    }
}
