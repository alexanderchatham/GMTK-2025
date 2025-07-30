using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public GameObject interactionPrompt;
    public UnityEvent onInteract;
    public bool oneTimeUse = false;
    void Start()
    {
        interactionPrompt.SetActive(false);
    }
    public void Show()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }
    public void Hide()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    public void Interact()
    {
        onInteract?.Invoke();
        print("Interacted with: " + gameObject.name);
        if (oneTimeUse)
        {
            Destroy(this);
        }
    }
}
