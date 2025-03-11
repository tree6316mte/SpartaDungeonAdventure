using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillManager : MonoSingleton<SkillManager>
{
    public UISkill slot;

    public ItemData itemData;

    public void Start()
    {
        SetItem(itemData);
    }

    public void SetItem(ItemData _itemData)
    {
        if (slot != null) slot.Clear();

        slot.Init(_itemData);
    }

    // UI 정보 새로고침
    public void ItemUse()
    {
        if (slot.item != null && !slot.isUsing)
        {
            slot.StartCoolTime(slot.item.duringTime);
        }
    }

}
