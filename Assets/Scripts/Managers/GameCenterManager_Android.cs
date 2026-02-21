using System;
using UnityEngine;


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

#if UNITY_ANDROID

    const string LeaderboardId = "CgkIpczdyKoREAIQAg";

    public bool IsAuthenticated => _authenticated;

     public void RequestSignIn(Action<bool> onComplete = null) {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(status => {
            if (status == SignInStatus.Success) {
                Debug.Log("Signed in successfully");
                // Enable leaderboards, submit pending scores, update UI
                _authenticated = true;
                onComplete?.Invoke(true);
            } else {
                Debug.Log("Sign-in dismissed or failed");
                // Stay in local-only mode, hide leaderboard prompts
                onComplete?.Invoke(false);
            }
        });
    }


    /// <summary>Authenticate the local user with Game Center on iOS. Called automatically on Start.</summary>
    public void AuthenticateAndroid(Action<bool> onComplete = null)
    {
        Debug.Log("[GameCenter] Authenticating Android");
        if (_authenticated)
        {
            onComplete?.Invoke(true);
            return;
        }

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        LoginGooglePlayGames(onComplete);
    }

    private void LoginGooglePlayGames(Action<bool> onComplete = null)
    {
        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            _authenticated = success == SignInStatus.Success;
            if (_authenticated){
                Debug.Log("[GameCenter] Authenticated: " + PlayGamesPlatform.Instance.GetUserId());
                PlayGamesPlatform.Instance.RequestServerSideAccess(
                    true, code => {
                        if (code != null){
                            m_GooglePlayGamesToken = code;
                        }
                    }
                );
            }
            Debug.Log("[GameCenter] Authenticated: " + _authenticated);
            Debug.Log("[GameCenter] Token: " + m_GooglePlayGamesToken);
            onComplete?.Invoke(_authenticated);
        });
    }

    public void ShowLeaderboardAndroid(Action onDismiss = null)
    {
        if (!_authenticated) {
            Debug.Log("[GameCenter] Not authenticated, requesting sign in");
            RequestSignIn(
                (success) => {
                    Debug.Log("[GameCenter] Sign in result: " + success);
                    if (success) {
                        ShowLeaderboardAndroid(onDismiss);
                    } else {
                        Debug.Log("[GameCenter] Sign in failed, showing leaderboard failed");
                        onDismiss?.Invoke();
                    }
                }
            );
        }
        else {
            Debug.Log("[GameCenter] Showing leaderboard Android");
            PlayGamesPlatform.Instance.SetDefaultLeaderboardForUI(LeaderboardId);
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
            onDismiss?.Invoke();
        }
    }

    // /// <summary>Report a score to the configured leaderboard. Only runs on iOS.</summary>
    // public void ReportScoreIOS(int level, Action<bool> onComplete = null)
    // {
    //     if (!_authenticated || string.IsNullOrEmpty(leaderboardId))
    //     {
    //         onComplete?.Invoke(false);
    //         return;
    //     }

    //     Social.ReportScore((long)level, leaderboardId, success =>
    //     {
    //         if (success)
    //             Debug.Log("[GameCenter] Score reported: " + level);
    //         else
    //             Debug.LogWarning("[GameCenter] Failed to report score.");
    //         onComplete?.Invoke(success);
    //     });
    // }

    //     /// <summary>Report achievement progress (0.0–100.0). Only runs on iOS.</summary>
    // public void ReportAchievementIOS(string achievementId, double progressPercent, Action<bool> onComplete = null)
    // {
    //     if (!_authenticated || string.IsNullOrEmpty(achievementId))
    //     {
    //         onComplete?.Invoke(false);
    //         return;
    //     }

    //     Social.ReportProgress(achievementId, progressPercent / 100.0, success =>
    //     {
    //         if (success)
    //             Debug.Log("[GameCenter] Achievement reported: " + achievementId);
    //         else
    //             Debug.LogWarning("[GameCenter] Failed to report achievement: " + achievementId);
    //         onComplete?.Invoke(success);
    //     });
    // }

    // /// <summary>Show the Game Center leaderboard UI. Only runs on iOS.</summary>
    // public void ShowLeaderboardIOS(Action onDismiss = null)
    // {
    //     if (!_authenticated)
    //     {
    //         Authenticate(success =>
    //         {
    //             if (success)
    //                 ShowLeaderboard(onDismiss);
    //             else
    //                 onDismiss?.Invoke();
    //         });
    //         return;
    //     }

    //     Social.ShowLeaderboardUI();
    //     onDismiss?.Invoke();
    // }

    // /// <summary>Show the Game Center achievements UI. Only runs on iOS.</summary>
    // public void ShowAchievementsIOS(Action onDismiss = null)
    // {
    //     if (!_authenticated)
    //     {
    //         Authenticate(success =>
    //         {
    //             if (success)
    //                 ShowAchievements(onDismiss);
    //             else
    //                 onDismiss?.Invoke();
    //         });
    //         return;
    //     }

    //     Social.ShowAchievementsUI();
    //     onDismiss?.Invoke();
    // }

#endif

}
