using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DescriptionHandler : StaticInstance<DescriptionHandler>
{
    [SerializeField] private GameObject descriptionWindow;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI nameText;

    public void ShowDescription(string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;
        descriptionWindow.SetActive(true);
    }

    public void HideDescription()
    {
        descriptionWindow.SetActive(false);
    }
}
