using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public static bool Paused { get; set; } = false;
    public static float Volume { get; set; } = 1.0f;
    public static bool GameOver { get; set; } = false;

    public static void TogglePause()
    {
        Paused = !Paused;
        Time.timeScale = Paused ? 0 : 1;
        Debug.Log("Game " + (Paused ? "Paused" : "Resumed"));
    }

    public static void SetVolume(float volume)
    {
        Volume = Mathf.Clamp01(volume);
        AudioListener.volume = Volume;
        Debug.Log("Volume set to: " + Volume);
    }
    public static void EndGame()
    {
        GameOver = true;
        Debug.Log("Game Over");
    }
    public static void ResetGame()
    {
        //Reload current scene or reset game state
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
