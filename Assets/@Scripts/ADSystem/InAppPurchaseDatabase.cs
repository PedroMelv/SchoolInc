using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InAppPurchaseDatabase", menuName = "StaticScriptableObjects/IAPDatabase")]
public class InAppPurchaseDatabase : SingletonObject<InAppPurchaseDatabase>
{
    public enum IAPType
    {
        COIN,
        SERVICE_A,
        SERVICE_B,
        SERVICE_C,
        SERVICE_D,
        SERVICE_E
    }
    
    public string coinName;
    public Texture2D coinSprite;

    public InAppPurchaseData[] InAppPurchases;

    [System.Serializable]
    public struct InAppPurchaseData
    {
        public IAPType type;
        public string content;
        public ulong quantity;

        public ulong price;
    }
}
