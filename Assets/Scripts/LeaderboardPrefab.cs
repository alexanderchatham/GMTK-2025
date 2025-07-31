using TMPro;
using UnityEngine;

public class LeaderboardPrefab : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI username;
    public TextMeshProUGUI placement;

    public void SetPrefab(string score, string username, string placement)
    {
        this.score.text = score;
        this.username.text = username;
        this.placement.text = placement;
    }
}
