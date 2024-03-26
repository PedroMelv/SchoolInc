using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BannerSpacing : MonoBehaviour
{
    [SerializeField] private float spacing = 200;
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        AdsSystem.Instance.OnShowingBannerChanged += OnBannerChanged;
    }

    private void OnBannerChanged(bool showing)
    {
        rectTransform.anchoredPosition = Vector2.down * (showing ? spacing : 0);
    }
}
