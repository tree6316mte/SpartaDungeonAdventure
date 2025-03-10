using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public Image coolImage;

    public GameObject tooltipUI; // 스킬 설명 UI
    public TextMeshProUGUI tooltipTitle; // 스킬 설명 UI
    public TextMeshProUGUI tooltipDesc; // 스킬 설명 UI

    public void Awake()
    {
        tooltipUI.SetActive(false); // 툴팁 UI 비활성화
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipUI.SetActive(true); // 마우스 올리면 툴팁 UI 활성화
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipUI.SetActive(false); // 마우스 나가면 툴팁 UI 비활성화
    }
}
