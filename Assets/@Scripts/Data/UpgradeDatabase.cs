using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "StaticScriptableObjects/UpgradeDatabase")]
public class UpgradeDatabase : SingletonObject<UpgradeDatabase>
{
    public Upgrade[] upgrades;

    [System.Serializable]
    public class Upgrade
    {
        public string name;
        public int maxQuantity;
        public ulong costBase;
        [Range(1f,1.5f)]public float rateGrowth;

        public BigInteger currentQuantity;
    }
}
