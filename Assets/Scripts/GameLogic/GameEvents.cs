using System;

public static class GameEvents
{
    public static Action OnPlayerDeath;
    public static Action OnTrackCleared;
    public static Action<int> OnDifficultyIncreased;
    public static Action<int> OnScoreUpdated;
    public static Action<StatusEffect, StatusEffect> OnEffectsHUDUpdated;
    public static Action OnRestartRequest;
}
