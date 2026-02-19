using System;
using UnityEngine;

#if UNITY_IOS
using UnityEngine.SocialPlatforms;
#pragma warning disable 0618 // Unity's Social API is deprecated but still the standard for Game Center on iOS
#endif

/// <summary>
/// Apple Game Center integration for iOS. All Game Center code is compiled and runs only on iOS.
/// On other platforms, public methods are no-ops. Configure leaderboard and achievement IDs
/// in App Store Connect and set the constants below (or make them configurable).
/// </summary>
public class GameCenterManager : MonoBehaviour
{
    public static GameCenterManager Instance { get; private set; }

    [Header("Game Center IDs (set in App Store Connect)")]
    [Tooltip("Leaderboard ID for session/high score (e.g. com.yourcompany.boardblast.bestscore)")]
    public string leaderboardId = "boardblast.highest_level";

    [Tooltip("Achievement: complete first level (optional)")]
    public string achievementFirstWin = "boardblast.first_win";

    [Tooltip("Achievement: reach level 5 (optional)")]
    public string achievementLevel5 = "boardblast.level_5";

    bool _authenticated;

#if UNITY_IOS
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
        if (Instance == this)
            Instance = null;
    }

    /// <summary>Authenticate the local user with Game Center. Called automatically on Start.</summary>
    public void Authenticate(Action<bool> onComplete = null)
    {
        if (_authenticated)
        {
            onComplete?.Invoke(true);
            return;
        }

        Social.localUser.Authenticate(success =>
        {
            _authenticated = success;
            if (success)
                Debug.Log("[GameCenter] Authenticated: " + Social.localUser.userName);
            else
                Debug.LogWarning("[GameCenter] Authentication failed.");
            onComplete?.Invoke(success);
        });
    }

    /// <summary>Report a score to the configured leaderboard. Only runs on iOS.</summary>
    public void ReportScore(int level, Action<bool> onComplete = null)
    {
        if (!_authenticated || string.IsNullOrEmpty(leaderboardId))
        {
            onComplete?.Invoke(false);
            return;
        }

        Social.ReportScore((long)level, leaderboardId, success =>
        {
            if (success)
                Debug.Log("[GameCenter] Score reported: " + level);
            else
                Debug.LogWarning("[GameCenter] Failed to report score.");
            onComplete?.Invoke(success);
        });
    }

    /// <summary>Report achievement progress (0.0–100.0). Only runs on iOS.</summary>
    public void ReportAchievement(string achievementId, double progressPercent, Action<bool> onComplete = null)
    {
        if (!_authenticated || string.IsNullOrEmpty(achievementId))
        {
            onComplete?.Invoke(false);
            return;
        }

        Social.ReportProgress(achievementId, progressPercent / 100.0, success =>
        {
            if (success)
                Debug.Log("[GameCenter] Achievement reported: " + achievementId);
            else
                Debug.LogWarning("[GameCenter] Failed to report achievement: " + achievementId);
            onComplete?.Invoke(success);
        });
    }

    /// <summary>Show the Game Center leaderboard UI. Only runs on iOS.</summary>
    public void ShowLeaderboard(Action onDismiss = null)
    {
        if (!_authenticated)
        {
            Authenticate(success =>
            {
                if (success)
                    ShowLeaderboard(onDismiss);
                else
                    onDismiss?.Invoke();
            });
            return;
        }

        Social.ShowLeaderboardUI();
        onDismiss?.Invoke();
    }

    /// <summary>Show the Game Center achievements UI. Only runs on iOS.</summary>
    public void ShowAchievements(Action onDismiss = null)
    {
        if (!_authenticated)
        {
            Authenticate(success =>
            {
                if (success)
                    ShowAchievements(onDismiss);
                else
                    onDismiss?.Invoke();
            });
            return;
        }

        Social.ShowAchievementsUI();
        onDismiss?.Invoke();
    }

    public bool IsAuthenticated => _authenticated;

#else
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

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Authenticate(Action<bool> onComplete = null) => onComplete?.Invoke(false);
    public void ReportScore(long score, Action<bool> onComplete = null) => onComplete?.Invoke(false);
    public void ReportAchievement(string achievementId, double progressPercent, Action<bool> onComplete = null) => onComplete?.Invoke(false);
    public void ShowLeaderboard(Action onDismiss = null) => onDismiss?.Invoke();
    public void ShowAchievements(Action onDismiss = null) => onDismiss?.Invoke();
    public bool IsAuthenticated => false;
#endif
}
