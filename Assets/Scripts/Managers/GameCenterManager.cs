using System;
using UnityEngine;

#if UNITY_IOS
using UnityEngine.SocialPlatforms;
#pragma warning disable 0618 // Unity's Social API is deprecated but still the standard for Game Center on iOS
#endif

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
#endif

/// <summary>
/// Apple Game Center integration for iOS. All Game Center code is compiled and runs only on iOS.
/// On other platforms, public methods are no-ops. Configure leaderboard and achievement IDs
/// in App Store Connect and set the constants below (or make them configurable).
/// </summary>
public partial class GameCenterManager : MonoBehaviour
{
    public static GameCenterManager Instance { get; private set; }
    private string m_GooglePlayGamesToken;

    [Header("Game Center IDs (set in App Store Connect)")]
    [Tooltip("Leaderboard ID for session/high score (e.g. com.yourcompany.boardblast.bestscore)")]
    public string leaderboardId = "boardblast.highest_level";

    [Tooltip("Achievement: complete first level (optional)")]
    public string achievementFirstWin = "boardblast.first_win";

    [Tooltip("Achievement: reach level 5 (optional)")]
    public string achievementLevel5 = "boardblast.level_5";

    bool _authenticated;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Authenticate();
    }

    void OnDestroy()
    {
        if (Instance == this){
            Instance = null;
        }
    }


    public void Authenticate(Action<bool> onComplete = null)
    {
        #if UNITY_IOS
        AuthenticateIOS(onComplete);
        #elif UNITY_ANDROID
        AuthenticateAndroid(onComplete);
        #else
        onComplete?.Invoke(false);
        #endif
    }

    /// <summary>Report a score to the configured leaderboard. Only runs on iOS.</summary>
    public void ReportScore(int level, Action<bool> onComplete = null)
    {
        #if UNITY_IOS
        ReportScoreIOS(level, onComplete);
        #else
        onComplete?.Invoke(false);
        #endif
    }

    /// <summary>Report achievement progress (0.0–100.0). Only runs on iOS.</summary>
    public void ReportAchievement(string achievementId, double progressPercent, Action<bool> onComplete = null)
    {
        #if UNITY_IOS
        ReportAchievementIOS(achievementId, progressPercent, onComplete);
        #else
        onComplete?.Invoke(false);
        #endif
    }

    public void ShowLeaderboard(Action onDismiss = null)
    {
        #if UNITY_IOS
        ShowLeaderboardIOS(onDismiss);
        #elif UNITY_ANDROID
        ShowLeaderboardAndroid(onDismiss);
        #else
        onDismiss?.Invoke();
        #endif
    }

    /// <summary>Show the Game Center achievements UI. Only runs on iOS.</summary>
    public void ShowAchievements(Action onDismiss = null)
    {
        #if UNITY_IOS
        ShowAchievementsIOS(onDismiss);
        #else
        onDismiss?.Invoke();
        #endif
    }


#if !UNITY_IOS && !UNITY_ANDROID
    public bool IsAuthenticated => false;
#endif

}
