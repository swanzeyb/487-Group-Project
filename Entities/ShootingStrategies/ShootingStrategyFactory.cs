namespace Entities.ShootingStrategies;

public static class ShootingStrategyFactory
{
    /// <summary>
    /// Creates a shooting strategy from a string pattern name.
    /// If the pattern is empty or unrecognised, falls back to the default for the given enemy type.
    /// </summary>
    public static IShootingStrategy Create(string attackPattern, EnemyType fallbackType)
    {
        string normalized = attackPattern?.Trim().ToLowerInvariant() ?? "";
        return normalized switch
        {
            "randomscatter" or "scatter" => new RandomScatterStrategy(damage: 5),
            "targeted"                   => new TargetedStrategy(damage: 5),
            "automatic"                  => new AutomaticFireStrategy(damage: 10),
            "laser"                      => new LaserBeamStrategy(),
            _                            => CreateDefault(fallbackType)
        };
    }

    private static IShootingStrategy CreateDefault(EnemyType type) => type switch
    {
        EnemyType.BetterGrunt => new TargetedStrategy(damage: 5),
        EnemyType.MidBoss     => new AutomaticFireStrategy(damage: 10),
        EnemyType.FinalBoss   => new LaserBeamStrategy(),
        _                     => new RandomScatterStrategy(damage: 5)
    };
}
