using System;
using UnityEngine;

public static class GameEvents
{
    public static Action OnPlayerDeath;
    public static Action OnTrackCleared;
    public static Action<int> OnDifficultyIncreased;
    public static Action<int> OnScoreUpdated;
    public static Action<Sprite, Sprite, bool> OnEffectsHUDUpdated;
    public static Action OnRestartRequest;
    public static Action OnPauseRequested;
    public static Action<bool> OnPauseStateChanged;
    public static Action<int> OnPlayerDodge;

    public static Action OnSaveRequest;
    public static Action<GameStateData> OnGatherSaveData;
    public static Action<GameStateData> OnRestoreSaveData;
}
