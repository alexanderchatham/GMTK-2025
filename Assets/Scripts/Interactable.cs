using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public GameObject interactionPrompt;
    public UnityEvent onInteract;
    public bool oneTimeUse = false;
    public bool interactOnTouch = false; // If true, will interact when touched
    void Start()
    {
        interactionPrompt.SetActive(false);
    }
    public void Show()
    {
        if (interactOnTouch)
        {
            Interact(); // Automatically interact if set to do so on touch
            return;
        }
         // Show the interaction prompt
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
        var anim = GetComponent<Animator>();
        var audio = GetComponent<AudioSource>();
        if(anim != null)
        {
            anim.SetTrigger("switch");
        }
        if(audio != null)
        {
            audio.Play();
        }
    }
}
