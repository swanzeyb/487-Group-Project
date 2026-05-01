using System.Collections.Generic;

namespace Screens;

public class TestSelectionLogic
{
    public static readonly IReadOnlyList<string> EnemyTypes =
        new[] { "Grunt", "BetterGrunt", "MidBoss", "FinalBoss" };

    public static readonly IReadOnlyList<string> MovementPatterns =
        new[] { "linear", "sinusoidal", "circular" };

    public static readonly IReadOnlyList<string> AttackPatterns =
        new[] { "default", "randomscatter", "targeted", "automatic", "laser" };

    // Row 0 = EnemyType, 1 = Movement, 2 = Attack, 3 = EnterGame
    public int FocusedRow { get; private set; } = 0;

    public int EnemyTypeIndex { get; private set; } = 0;
    public int MovementIndex  { get; private set; } = 0;
    public int AttackIndex    { get; private set; } = 0;

    public string SelectedEnemyType => EnemyTypes[EnemyTypeIndex];
    public string SelectedMovement  => MovementPatterns[MovementIndex];
    public string SelectedAttack    => AttackPatterns[AttackIndex];

    private const int TotalRows = 4;

    public void MoveUp()   => FocusedRow = (FocusedRow - 1 + TotalRows) % TotalRows;
    public void MoveDown() => FocusedRow = (FocusedRow + 1) % TotalRows;

    public void CycleLeft()
    {
        switch (FocusedRow)
        {
            case 0: EnemyTypeIndex = (EnemyTypeIndex - 1 + EnemyTypes.Count)       % EnemyTypes.Count;       break;
            case 1: MovementIndex  = (MovementIndex  - 1 + MovementPatterns.Count) % MovementPatterns.Count; break;
            case 2: AttackIndex    = (AttackIndex    - 1 + AttackPatterns.Count)   % AttackPatterns.Count;   break;
        }
    }

    public void CycleRight()
    {
        switch (FocusedRow)
        {
            case 0: EnemyTypeIndex = (EnemyTypeIndex + 1) % EnemyTypes.Count;       break;
            case 1: MovementIndex  = (MovementIndex  + 1) % MovementPatterns.Count; break;
            case 2: AttackIndex    = (AttackIndex    + 1) % AttackPatterns.Count;   break;
        }
    }

    /// <returns>true when the user confirms on the EnterGame row.</returns>
    public bool Confirm() => FocusedRow == 3;
}
