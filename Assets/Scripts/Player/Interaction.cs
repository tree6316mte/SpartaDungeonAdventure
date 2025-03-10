using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance = 30;

    public GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public TextMeshProUGUI promptText;
    public GameObject promptBackground;
    private Camera camera;


    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    if (hit.collider.TryGetComponent(out IInteractable interactable))
                    {
                        ShowDescription(hit, interactable);
                    }
                    else
                    {
                        HideDescription();
                    }
                }
            }
            else
            {
                HideDescription();
            }
        }
    }

    private void ShowDescription(RaycastHit hit, IInteractable interactable)
    {
        curInteractGameObject = hit.collider.gameObject;
        curInteractable = interactable;
        interactable.ShowOutline();
        SetPromptText();
    }
    private void HideDescription()
    {
        curInteractGameObject = null;
        if (curInteractable != null)
        {
            curInteractable.HideOutline();
        }
        curInteractable = null;
        promptBackground.SetActive(false);
    }

    private void SetPromptText()
    {
        promptBackground.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}