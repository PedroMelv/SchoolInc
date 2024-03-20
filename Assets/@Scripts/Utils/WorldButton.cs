using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class WorldButton : MonoBehaviour
{
    [SerializeField] private bool interactable = true;
    public bool Interactable 
    {  get { return interactable; } 
        set 
        {
            if (value == false) CurrentColor = disabledColor;
            interactable = value; 
        } 
    }

    [SerializeField] private new SpriteRenderer renderer;

    private Color baseColor;

    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color pressedColor = new Color(.85f, .85f, .85f,1f);
    [SerializeField] private Color disabledColor = new Color(.5f, .5f, .5f, .85f);

    private Color CurrentColor { get => currentColor;
        set 
        { 
            currentColor = value;
            ChangeColor();
        } 
    }

    private Color currentColor;

    private Coroutine colorChanging;


    public Action OnButtonPressed;

    private void Awake()
    {
        if (renderer == null) renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        baseColor = renderer.color;
        CurrentColor = interactable ? defaultColor : disabledColor;
    }



    private void OnMouseDown()
    {
        CurrentColor = pressedColor;
        OnButtonPressed?.Invoke();
    }

    public void OnMouseUp()
    {
        CurrentColor = defaultColor;
    }

    private void ChangeColor()
    {
        renderer.DOKill(true);
        if(colorChanging != null)
        {
            StopCoroutine(colorChanging);
        }
        colorChanging = StartCoroutine(EChangeColor());
    }

    private IEnumerator EChangeColor()
    {
        bool completed = false;
        renderer.DOColor(MultiplyColor(), .05f).onComplete += () => completed = true;
        while(!completed)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private Color MultiplyColor()
    {
        Color color = new Color(0f, 0f, 0f, 0f);
        color.r = (currentColor + baseColor).r / 2;
        color.g = (currentColor + baseColor).g / 2;
        color.b = (currentColor + baseColor).b / 2;
        color.a = currentColor.a > baseColor.a ? baseColor.a : currentColor.a;

        return color;
    }
}
