using Microsoft.Xna.Framework;
using Entities;

namespace Core;

public static class GameConfig
{
    // Window Size
    public const int ScreenWidth = 960;
    public const int ScreenHeight = 540;

    // Playfield bounds (Touhou-style: playfield on left, panel on right)
    public const int PlayfieldLeft = 20;
    public const int PlayfieldTop = 20;
    public const int PlayfieldWidth = 620;
    public const int PlayfieldHeight = 500;

    public static readonly Rectangle Playfield = new Rectangle(PlayfieldLeft, PlayfieldTop, PlayfieldWidth, PlayfieldHeight);

    // Right-side HUD panel (20px padding on right edge)
    public const int PanelX = 660;
    public const int PanelWidth = 280;

    public const float PlayerSpeedNormal = 260f;
    public const float PlayerSpeedSlow = 130f;
    public const float BulletSpeedDefault = 180f;

    // Debug/Test Mode settings
    public static bool IsDebugMode { get; set; } = false;
    public static EnemyType SelectedEnemyType { get; set; } = EnemyType.Grunt;
}