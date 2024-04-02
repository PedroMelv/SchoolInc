using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoreUI<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected InputHandler inputHandler;
    [SerializeField] protected Transform storeContainer;
    [SerializeField] protected TextMeshProUGUI tierPrefab;
    [SerializeField] protected BuyNode buyPrefab;

    protected List<BuyNode> nodesOnScreen = new List<BuyNode>();

    public virtual void InitializeArea()
    {
        UpdateStoreContainer();
    }
    public virtual void CloseArea()
    {

    }
    public virtual void UpdateStoreContainer()
    {
        if (nodesOnScreen.Count > 0)
        {
            UpdateBuyNodes();
            return;
        }

        InitializeBuyNodes();
    }


    protected virtual void InitializeBuyNodes()
    {
        
    }
    protected virtual void UpdateBuyNodes()
    {
        
    }
}
