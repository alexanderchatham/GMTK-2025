using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        //assume 2 waypoints for simplicity use sin speed and time to lerp the platform between them
        if (waypoints.Length >= 2)
        {
            float t = Mathf.PingPong(Time.time * speed, 1f);
            transform.position = Vector3.Lerp(waypoints[0].position, waypoints[1].position, t);
        }
        else
        {
            Debug.LogWarning("Not enough waypoints set for MovingPlatform.");
        }
        
    }
}
