using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OnlyLightWhenPlayerNear : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Light"))
        {
            collision.gameObject.GetComponentInChildren<Light2D>().enabled = true; // Disable light when exiting the trigger
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Light"))
        {
            collision.gameObject.GetComponentInChildren<Light2D>().enabled = false; // Disable light when exiting the triggerFV
        }
    }
}
