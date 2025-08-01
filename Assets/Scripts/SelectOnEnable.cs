using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Selectable))]
public class SelectOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        // Check if the GameObject has a Selectable component
        Selectable selectable = GetComponent<Selectable>();
        if (selectable != null)
        {
            // Select the GameObject
            selectable.Select();
        }
    }
}
