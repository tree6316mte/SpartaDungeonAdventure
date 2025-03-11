using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item;   // 아이템 데이터

    public Image iconImage;
    public Image coolImage;

    public GameObject tooltipUI; // 스킬 설명 UI
    public TextMeshProUGUI tooltipDesc; // 스킬 설명 UI

    public bool isUsing = false;

    public void Init(ItemData _item)
    {
        Clear();

        item = _item;
        tooltipDesc.text = $"{item.DisplayName}\n{item.Description}";
        iconImage.sprite = item.sprite;
    }

    public void Clear()
    {
        SkillDisable();
        item = null;
        tooltipDesc.text = $""; ;
        iconImage.sprite = null;
        isUsing = false;

        coolImage.fillAmount = 0;
        isUsing = false;
        StopAllCoroutines();
    }

    public void StartCoolTime(float coolDown)
    {
        StartCoroutine(CoStartCoolTime(coolDown));
    }

    public IEnumerator CoStartCoolTime(float coolDown)
    {
        float currentCoolDown = 0;
        isUsing = true;
        SkillEnable();
        while (item != null && currentCoolDown <= coolDown)
        {
            currentCoolDown += Time.deltaTime;
            ImageFillAmount(coolDown, currentCoolDown);
            yield return null;
        }
        SkillDisable();
        isUsing = false;
        // Clear();
    }

    public void SkillEnable()
    {
        if (item == null) return;
        if (item.itemType == ItemType.JumpUp)
        {
            GameManager.Instance.Player.jumpForce = 10f;
        }
        else if (item.itemType == ItemType.SpeedUp)
        {
            GameManager.Instance.Player.runSpeed = 12f;
        }
    }
    public void SkillDisable()
    {
        if (item == null) return;
        if (item.itemType == ItemType.JumpUp)
        {
            GameManager.Instance.Player.jumpForce = 4f;
        }
        else if (item.itemType == ItemType.SpeedUp)
        {
            GameManager.Instance.Player.runSpeed = 6f;
        }
    }
    public void ImageFillAmount(float coolDown, float currentCoolDown)
    {
        coolImage.fillAmount = 1 - currentCoolDown / coolDown;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            tooltipUI.SetActive(true); // 마우스 올리면 툴팁 UI 활성화
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipUI.SetActive(false); // 마우스 나가면 툴팁 UI 비활성화
    }
}
