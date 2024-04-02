using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Observable<T>
{
    [SerializeField] private T value;
    [SerializeField] private UnityEvent<T> onValueChanged;

    public T Value
    {
        get => value;
        set => Set(value);
    }

    public static implicit operator T(Observable<T> observable) => observable.value;

    public Observable(T Value, UnityAction<T> callback = null)
    {
        this.value = Value;
        this.onValueChanged = new UnityEvent<T>();
        if(callback != null)this.onValueChanged.AddListener(callback);
    }
    public void Set(T value)
    {
        if(Equals(this.value, value)) return;
        this.value = value;
        Invoke();
    }

    public void Invoke()
    {

        onValueChanged.Invoke(value);
    }

    public void AddListener(UnityAction<T> callback)
    {
        if (callback == null) return;
        if(onValueChanged == null) return;

        onValueChanged.AddListener(callback);
    }

    public void RemoveListener(UnityAction<T> callback)
    {
        if (callback == null) return;
        if(onValueChanged == null) return;

        onValueChanged.RemoveListener(callback);
    }

    public void RemoveAllListeners()
    {
        if(onValueChanged == null) return;

        onValueChanged.RemoveAllListeners();
    }

    public void Dispose()
    {
        RemoveAllListeners();
        onValueChanged = null;
        value = default;
    }
}
