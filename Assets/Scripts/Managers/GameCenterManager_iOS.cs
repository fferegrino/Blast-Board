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
public partial class GameCenterManager : MonoBehaviour
{

#if UNITY_IOS

    public bool IsAuthenticated => _authenticated;


    /// <summary>Authenticate the local user with Game Center on iOS. Called automatically on Start.</summary>
    public void AuthenticateIOS(Action<bool> onComplete = null)
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
    public void ReportScoreIOS(int level, Action<bool> onComplete = null)
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
    public void ReportAchievementIOS(string achievementId, double progressPercent, Action<bool> onComplete = null)
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
    public void ShowLeaderboardIOS(Action onDismiss = null)
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
    public void ShowAchievementsIOS(Action onDismiss = null)
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

#endif

}
