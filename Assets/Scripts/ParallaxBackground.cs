using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform target; // Usually the player
    public float parallaxFactor = 0.5f; // Less than 1 for background to move slower

    private Vector3 previousTargetPosition;

    private void Start()
    {
        if (target == null)
        {
            target = Camera.main.transform;
        }

        previousTargetPosition = target.position;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = target.position - previousTargetPosition;
        transform.position -= deltaMovement * parallaxFactor;
        previousTargetPosition = target.position;
    }
}
