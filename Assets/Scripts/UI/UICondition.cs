using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition stamina;

    private void Start()
    {
        GameManager.Instance.Player.condition.uiCondition = this;
    }
}