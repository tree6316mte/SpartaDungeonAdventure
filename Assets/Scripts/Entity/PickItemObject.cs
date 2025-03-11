using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickItemObject : MonoBehaviour, IInteractable
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
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("???");
        SkillManager.Instance.SetItem(data);
        Destroy(gameObject);
    }
}
