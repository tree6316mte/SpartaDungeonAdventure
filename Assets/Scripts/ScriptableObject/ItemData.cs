using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
