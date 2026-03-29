using System;

namespace UI;

public class HudPanelData
{
    public int Lives { get; set; } = 3;
    public int Score { get; set; } = 0;
    public string PhaseName { get; set; } = "";

    private float _bossHealthPercent = 1f;
    public float BossHealthPercent
    {
        get => _bossHealthPercent;
        set => _bossHealthPercent = Math.Clamp(value, 0f, 1f);
    }

    public int BombCount { get; set; } = 0;

    // Add Player Health Tracking
    public int PlayerHealth { get; set; } = 100;
    public int PlayerMaxHealth { get; set; } = 100;
}