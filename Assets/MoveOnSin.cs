using UnityEngine;

public class MoveOnSin : MonoBehaviour
{
    Vector3 startPosition;
    void Start()
    {
        startPosition = transform.position; // Store the initial position of the object
    }
    void Update()
    {
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * 2) * 0.5f, 0); // Move the object up and down in a sine wave pattern
    }
}
