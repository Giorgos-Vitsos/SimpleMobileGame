using System;

public static class GameEvents
{
    public static Action OnPlayerDeath;
    public static Action OnTrackCleared;
    public static Action<int> OnDifficultyIncreased;
    public static Action<int> OnScoreUpdated;
    public static Action<StatusEffect, StatusEffect> OnEffectsHUDUpdated;
    public static Action OnRestartRequest;
    public static Action OnPauseRequested;
    public static Action<bool> OnPauseStateChanged;
    public static Action<int> OnPlayerDodge;
    public static Action<GameStateData> OnGatherSaveData;
    public static Action<GameStateData> OnRestoreSaveData;
}
