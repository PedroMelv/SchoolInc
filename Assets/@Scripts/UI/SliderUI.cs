using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SliderUI : MonoBehaviour
{
    private Transform fill;

    private void Awake()
    {
        
    }

    public void SetFill(float value)
    {
        if (fill == null) GetFill();
        fill.localScale = new Vector3(value, 1, 1);
    }

    public void SetFill(int current, int max)
    {
        if (fill == null) GetFill();
        fill.localScale = new Vector3((float)current / max, 1, 1);
    }

    private void GetFill()
    {
        fill = transform.Find("Fill");
        if (fill == null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name == "Fill")
                {
                    fill = child;
                    break;
                }
            }
        }


    }
}
