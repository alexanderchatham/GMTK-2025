using System.Collections;
using UnityEngine;

public class ShowCinemachineCamera : MonoBehaviour
{
    public GameObject cameraObject;
    public float showTime = 3f; // Duration to show the camera
    public void ShowCamera()
    {
        if (cameraObject != null)
        {
            cameraObject.SetActive(true);
            Debug.Log("Cinemachine Camera is now active.");
        }
        else
        {
            Debug.LogWarning("Camera object is not assigned.");
        }
    }
    public void showCameraWithDelay()
    {
        StartCoroutine(showCameraWithDelayCo());
    }
    IEnumerator showCameraWithDelayCo()
    {
        ShowCamera();
        yield return new WaitForSeconds(showTime);
        if (cameraObject != null)
        {
            cameraObject.SetActive(false);
            Debug.Log("Cinemachine Camera is now inactive.");
        }
    }
}
