using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }
    public float time;
    public TextMeshProUGUI timerText;
    public bool isRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (!isRunning)
        {
            return; // Exit if the timer is not running
        }
        if (!GameSettings.Paused && !GameSettings.GameOver)
        {
            time += Time.deltaTime;
            UpdateTimerText();
        }
    }
    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
