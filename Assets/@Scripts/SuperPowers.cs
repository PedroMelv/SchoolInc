using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPowers : StaticInstance<SuperPowers>, IBind<SuperPowersData>, ITimeListener
{
    public bool CanSuperTap { get { return superTap_enabled || superTap_duration > 0f; } }

    [field:SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    [Header("Super Tap")]
    [SerializeField]private bool superTap_enabled = false;
    [SerializeField]private float superTap_duration = 0f;
    [SerializeField] private Color superTapColor = Color.blue;
    [SerializeField] private Color tapColor = Color.red;

    [SerializeField] private UnityEngine.UI.Image tapButton;

    [SerializeField] private SuperPowersData data;

    private void Update()
    {
        superTap_duration -= Time.deltaTime;

        data.superTap_enabled = superTap_enabled;
        data.superTap_duration = superTap_duration;

        tapButton.color = CanSuperTap ? superTapColor : tapColor;
    }

    public void BuySuperTap()
    {
        superTap_enabled = true;
        data.superTap_enabled = superTap_enabled;

        SaveLoadSystem.Instance.SaveGame();
    }

    public void BuySuperTapDuration(float duration)
    {
        superTap_duration += duration;
    }

    public void Bind(SuperPowersData data)
    {
        this.data = data;
        Id = data.Id;

        superTap_enabled = data.superTap_enabled;
        superTap_duration = data.superTap_duration;
    }

    public void CalculateTime(double seconds)
    {
        superTap_duration -= (float)seconds;
    }
}

[System.Serializable]
public class SuperPowersData : ISaveable
{
    public bool superTap_enabled;
    public float superTap_duration;

    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    public void Reset()
    {
        
    }

    public void Reset_Ascended()
    {
        
    }
}
