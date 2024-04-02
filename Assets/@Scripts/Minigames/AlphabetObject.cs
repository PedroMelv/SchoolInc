using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlphabetObject : MonoBehaviour
{
    private TextMeshProUGUI alphabetText;
    private char letter;
    public char Letter => letter;

    public Observable<AlphabetObject> attachedOn = new Observable<AlphabetObject>(null);

    private HandsignDraggable draggable;
    public HandsignDraggable Draggable {
        get
        {
            if(draggable == null) draggable = GetComponent<HandsignDraggable>();
            return draggable;
        }
    }

    private RectTransform rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if(rectTransform == null) rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }

    private void GetComponents()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (draggable == null) draggable = GetComponent<HandsignDraggable>();
        if (alphabetText == null) alphabetText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialize(char letter, Vector3 position)
    {
        GetComponents();

        this.letter = letter;
        alphabetText.text = new string(letter,1);

        transform.localPosition = position;
    }

    public bool CompareLetters(AlphabetObject other)
    {
        return letter == other.Letter;
    }
}
