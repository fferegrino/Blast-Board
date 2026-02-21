using System;
using UnityEngine;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
#pragma warning disable 0618 // Unity Social API is deprecated; still used by Play Games plugin
#endif

/// <summary>
/// Google Play Games integration for Android. Score/achievement reporting and UI are implemented here;
/// the base GameCenterManager routes public API calls to this partial on Android.
/// </summary>
public partial class GameCenterManager : MonoBehaviour
{
#if UNITY_ANDROID

    public bool IsAuthenticated => _authenticated;

    string GetLeaderboardId() =>
        string.IsNullOrEmpty(leaderboardIdAndroid) ? leaderboardId : leaderboardIdAndroid;

    /// <summary>Manual sign-in (e.g. from a "Sign in" button). Start() uses Authenticate() automatically.</summary>
    public void RequestSignIn(Action<bool> onComplete = null)
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(status =>
        {
            if (status == SignInStatus.Success)
            {
                _authenticated = true;
                if (Debug.isDebugBuild)
                {
                    Debug.Log("[GameCenter] Signed in successfully.");
                }
                onComplete?.Invoke(true);
            }
            else
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("[GameCenter] Sign-in dismissed or failed.");
                }
                onComplete?.Invoke(false);
            }
        });
    }

    /// <summary>Authenticate with Google Play Games. Called automatically on Start.</summary>
    public void AuthenticateAndroid(Action<bool> onComplete = null)
    {
        if (_authenticated)
        {
            onComplete?.Invoke(true);
            return;
        }

        PlayGamesPlatform.DebugLogEnabled = Debug.isDebugBuild;
        PlayGamesPlatform.Activate();
        LoginGooglePlayGames(onComplete);
    }

    void LoginGooglePlayGames(Action<bool> onComplete = null)
    {
        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            _authenticated = success == SignInStatus.Success;
            if (_authenticated)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("[GameCenter] Authenticated: " + PlayGamesPlatform.Instance.GetUserId());
                }
                PlayGamesPlatform.Instance.RequestServerSideAccess(
                    true, code =>
                    {
                        if (code != null)
                        {
                            m_GooglePlayGamesToken = code;
                        }
                    });
            }
            else if (Debug.isDebugBuild)
            {
                Debug.LogWarning("[GameCenter] Authentication failed.");
            }
            onComplete?.Invoke(_authenticated);
        });
    }

    public void ReportScoreAndroid(int level, Action<bool> onComplete = null)
    {
        string id = GetLeaderboardId();
        if (!_authenticated || string.IsNullOrEmpty(id))
        {
            onComplete?.Invoke(false);
            return;
        }
        Social.ReportScore((long)level, id, success =>
        {
            if (Debug.isDebugBuild)
            {
                if (success)
                {
                    Debug.Log("[GameCenter] Score reported: " + level);
                }
                else
                {
                    Debug.LogWarning("[GameCenter] Failed to report score.");
                }
            }
            onComplete?.Invoke(success);
        });
    }

    public void ReportAchievementAndroid(string achievementId, double progressPercent, Action<bool> onComplete = null)
    {
        if (!_authenticated || string.IsNullOrEmpty(achievementId))
        {
            onComplete?.Invoke(false);
            return;
        }
        Social.ReportProgress(achievementId, progressPercent / 100.0, success =>
        {
            if (Debug.isDebugBuild)
            {
                if (success)
                {
                    Debug.Log("[GameCenter] Achievement reported: " + achievementId);
                }
                else
                {
                    Debug.LogWarning("[GameCenter] Failed to report achievement: " + achievementId);
                }
            }
            onComplete?.Invoke(success);
        });
    }

    public void ShowLeaderboardAndroid(Action onDismiss = null)
    {
        if (!_authenticated)
        {
            RequestSignIn(success =>
            {
                if (success)
                {
                    ShowLeaderboardAndroid(onDismiss);
                }
                else
                {
                    onDismiss?.Invoke();
                }
            });
            return;
        }
        PlayGamesPlatform.Instance.SetDefaultLeaderboardForUI(GetLeaderboardId());
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
        onDismiss?.Invoke();
    }

    public void ShowAchievementsAndroid(Action onDismiss = null)
    {
        if (!_authenticated)
        {
            RequestSignIn(success =>
            {
                if (success)
                {
                    ShowAchievementsAndroid(onDismiss);
                }
                else
                {
                    onDismiss?.Invoke();
                }
            });
            return;
        }
        PlayGamesPlatform.Instance.ShowAchievementsUI();
        onDismiss?.Invoke();
    }

#endif
}
