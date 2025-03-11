using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ItemType
{
    None,
    SpeedUp,
    JumpUp
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [SerializeField]
    private string displayName;
    public string DisplayName
    {
        get { return displayName; }
    }
    [SerializeField]
    private string description;
    public string Description
    {
        get { return description; }
    }

    public ItemType itemType = ItemType.None;
    public Sprite sprite;

    public float duringTime = 10f;
}
