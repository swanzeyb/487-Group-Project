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
    public static Enemy Create(SimpleDrawer drawer, EnemyType type, Vector2 position, Vector2 velocity, BulletManager bulletManager)
    {
        IShootingStrategy strategy = type switch
        {
            EnemyType.Grunt => new RandomScatterStrategy(5),
            EnemyType.BetterGrunt => new TargetedStrategy(10),
            EnemyType.MidBoss => new AutomaticFireStrategy(12),
            EnemyType.FinalBoss => new LaserBeamStrategy(),
            _ => new RandomScatterStrategy(5)
        };

        return new Enemy(drawer, type, position, velocity, strategy, bulletManager);
    }
}