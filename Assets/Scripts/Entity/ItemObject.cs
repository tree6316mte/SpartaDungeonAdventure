using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public string GetInteractPrompt();
    public void ShowOutline();
    public void HideOutline();
    public void OnInteract();
}


public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;
    [SerializeField]
    private List<Outline> outlines;

    public string GetInteractPrompt()
    {
        string str = $"{data.DisplayName}\n{data.Description}";
        return str;
    }

    public void ShowOutline()
    {
        foreach (var outline in outlines)
        {
            outline.enabled = true;
        }
    }

    public void HideOutline()
    {
        foreach (var outline in outlines)
        {
            outline.enabled = false;
        }
    }

    public void OnInteract()
    {
        //Player 스크립트 먼저 수정
        // CharacterManager.Instance.Player.itemData = data;
        // CharacterManager.Instance.Player.addItem?.Invoke();
        // Destroy(gameObject);
    }
}