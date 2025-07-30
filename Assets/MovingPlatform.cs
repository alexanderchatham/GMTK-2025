using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] waypoints;
    //inverse movement to make platforms move in the opposite direction
    //this is useful for platforms that move in a loop or back and forth
    public bool inverse;
    public float speed = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        //assume 2 waypoints for simplicity use sin speed and time to lerp the platform between them
        if (waypoints.Length >= 2)
        {
            float t = Mathf.PingPong(Time.time * speed, 1f);
            if(inverse)
                t = 1f - t; // Inverse movement logic
            transform.position = Vector3.Lerp(waypoints[0].position, waypoints[1].position, t);
        }
        else
        {
            Debug.LogWarning("Not enough waypoints set for MovingPlatform.");
        }
        
    }
}
