using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    public float minRotationSpeed = 100f; // Minimum rotation speed in degrees per second
    public float maxRotationSpeed = 400f; // Maximum rotation speed in degrees per second
    private float rotationSpeed = 10f; // Degrees per second
    private void Start()
    {
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
    }
    public void Update()
    {
        // Calculate the rotation amount based on the speed and time since last frame
        float rotationAmount = rotationSpeed * Time.deltaTime;
        // Apply the rotation to the transform of the GameObject
        transform.Rotate(Vector3.forward, rotationAmount);
    }
}