using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartingScreen : MonoBehaviour
{
    [Header("Text Sequence Settings")]
    public List<TextMeshProUGUI> textElements;
    public float fadeDuration = 1.5f;
    public float delayBetweenTexts = 0.5f;
    public static StartingScreen Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
    private void Start()
    {
        foreach (var text in textElements)
        {
            SetAlpha(text, 0f);
        }

        StartCoroutine(FadeInSequence());
    }

    private IEnumerator FadeInSequence()
    {
        foreach (var text in textElements)
        {
            yield return StartCoroutine(FadeTextIn(text));
            yield return StartCoroutine(FadeTextOut(text));
            yield return new WaitForSeconds(delayBetweenTexts);
        }
        yield return StartCoroutine(FadeOutCanvasGroup(GetComponent<CanvasGroup>()));
        GameTimer.Instance.isRunning = true; // Reset the game timer at the start of the game
    }
    private IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            canvasGroup.alpha = alpha;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.alpha = 0f; // ensure fully invisible
    }
    private IEnumerator FadeTextOut(TextMeshProUGUI text)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            SetAlpha(text, alpha);
            yield return new WaitForEndOfFrame();
        }
        SetAlpha(text, 0f); // ensure fully invisible
    }
    private IEnumerator FadeTextIn(TextMeshProUGUI text)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            SetAlpha(text, alpha);
            yield return new WaitForEndOfFrame();
        }
        SetAlpha(text, 1f); // ensure fully visible
    }

    private void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
    public void OnJump()
    {
        StopAllCoroutines(); // Stop any ongoing fade animations
        GameSettings.Paused = false; // Unpause the game when the jump button is pressed
        GameSettings.GameOver = false; // Reset game over state
        GameTimer.Instance.isRunning = true; // Start the game timer
        Destroy(gameObject); // Remove the starting screen
    }
}
