using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject LeaderboardPrefab;
    public LeaderboardPrefab PlayerLeaderboardReference;
    public Transform LeaderboardContainer;
    const string LeaderboardId = "leaderboard";
    string VersionId { get; set; }
    int Offset { get; set; }
    int Limit { get; set; }
    int RangeLimit { get; set; }
    List<string> FriendIds { get; set; }

    async void Awake()
    {
        await UnityServices.InitializeAsync();

        await SignInAnonymously();
    }

    async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
        };
        if(!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        GetPlayerScore();
        GetPaginatedScores();
    }




    public async void AddScore()
    {
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, GameTimer.Instance.time);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        GetPlayerScore();
        GetScores();
    }

    public async void GetScores()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        for (int i = LeaderboardContainer.childCount; i > 0; i--)
            Destroy(LeaderboardContainer.GetChild(i-1).gameObject);
        
        foreach (var response in scoresResponse.Results)
        {
            float time = (float)response.Score;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            var lb = Instantiate(LeaderboardPrefab, LeaderboardContainer);
            lb.GetComponent<LeaderboardPrefab>().SetPrefab(string.Format("{0:00}:{1:00}", minutes, seconds), response.PlayerName, (response.Rank + 1).ToString());
        }
    
    }

    public async void GetPaginatedScores()
    {
        Offset = 0;
        Limit = 10;
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Offset = Offset, Limit = Limit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        for (int i = LeaderboardContainer.childCount; i > 0; i--)
            Destroy(LeaderboardContainer.GetChild(i-1).gameObject);
        
        foreach (var response in scoresResponse.Results)
        {
        float time = (float)response.Score;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        var lb = Instantiate(LeaderboardPrefab, LeaderboardContainer);
            lb.GetComponent<LeaderboardPrefab>().SetPrefab(string.Format("{0:00}:{1:00}", minutes, seconds), response.PlayerName, (response.Rank+1).ToString());
        }
    }

    public async void GetPlayerScore()
    {
        try
        {

            var scoreResponse =
                await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
            Debug.Log(JsonConvert.SerializeObject(scoreResponse));
            float time = (float)scoreResponse.Score;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            PlayerLeaderboardReference.SetPrefab(string.Format("{0:00}:{1:00}", minutes, seconds), scoreResponse.PlayerName, (scoreResponse.Rank+1).ToString());
        }
        catch
        {
            
            PlayerLeaderboardReference.SetPrefab("","Finish Game to be added", "");
        }
    }

    public async void GetPlayerRange()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetPlayerRangeAsync(LeaderboardId, new GetPlayerRangeOptions{RangeLimit = RangeLimit});
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetScoresByPlayerIds()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresByPlayerIdsAsync(LeaderboardId, FriendIds);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    // If the Leaderboard has been reset and the existing scores were archived,
    // this call will return the list of archived versions available to read from,
    // in reverse chronological order (so e.g. the first entry is the archived version
    // containing the most recent scores)
    public async void GetVersions()
    {
        var versionResponse =
            await LeaderboardsService.Instance.GetVersionsAsync(LeaderboardId);

        // As an example, get the ID of the most recently archived Leaderboard version
        VersionId = versionResponse.Results[0].Id;
        Debug.Log(JsonConvert.SerializeObject(versionResponse));
    }

    public async void GetVersionScores()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetVersionScoresAsync(LeaderboardId, VersionId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetPaginatedVersionScores()
    {
        Offset = 10;
        Limit = 10;
        var scoresResponse =
            await LeaderboardsService.Instance.GetVersionScoresAsync(LeaderboardId, VersionId, new GetVersionScoresOptions { Offset = Offset, Limit = Limit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));

    }

    public async void GetPlayerVersionScore()
    {
        var scoreResponse =
            await LeaderboardsService.Instance.GetVersionPlayerScoreAsync(LeaderboardId, VersionId);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }
}
