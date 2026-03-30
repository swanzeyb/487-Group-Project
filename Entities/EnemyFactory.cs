using Core;
using Microsoft.Xna.Framework;
using Entities.ShootingStrategies;

namespace Entities;

/// <summary>
/// Centralised factory for creating Enemy instances.
/// Teammates adding HP, bullet patterns, etc. only need to modify this one place.
/// </summary>
public static class EnemyFactory
{
    public static Enemy Create(SimpleDrawer drawer, EnemyType type, Vector2 position, Vector2 velocity, BulletManager bulletManager, string movementPattern = "linear")
    {
        IShootingStrategy strategy = type switch
        {
            EnemyType.Grunt => new RandomScatterStrategy(damage: 5),
            EnemyType.BetterGrunt => new TargetedStrategy(damage: 5),
            EnemyType.MidBoss => new AutomaticFireStrategy(damage: 10),
            EnemyType.FinalBoss => new LaserBeamStrategy(),
            _ => new RandomScatterStrategy()
        };

        return new Enemy(drawer, type, position, velocity, strategy, bulletManager, movementPattern);
    }
}
