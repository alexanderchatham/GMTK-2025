using UnityEngine;

public class LavaSpitter : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform movingPlatform;
    //inverse movement to make platforms move in the opposite direction
    //this is useful for platforms that move in a loop or back and forth
    public bool inverse;
    public float speed = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool random = false; // If true, will randomly select waypoints to move between
    private void Start()
    {
        if (random)
        {
            inverse = Random.Range(0, 2) == 0; // Randomly set inverse to true or false
            speed = Random.Range(.05f, .2f); // Randomly set speed between 1 and 5
        }
    }

    // Update is called once per frame
    void Update()
    {
        //assume 2 waypoints for simplicity use sin speed and time to lerp the platform between them
        if (waypoints.Length >= 2)
        {
            float t = Mathf.Abs(Time.time * speed - transform.position.x/10) % 1;
            if (inverse)
                t = 1f - t; // Inverse movement logic
            movingPlatform.position = Vector3.Lerp(waypoints[0].position, waypoints[1].position, t);
        }
        else
        {
            Debug.LogWarning("Not enough waypoints set for MovingPlatform.");
        }

    }
    Renderer platformRenderer;
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        // Try to get renderer if not cached
        if (platformRenderer == null)
            platformRenderer = movingPlatform.GetComponent<Renderer>();


        Gizmos.color = Color.green;
        foreach (var point in waypoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position, movingPlatform.transform.localScale.x/2);
            }
        }
    }
}
