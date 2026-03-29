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
            EnemyType.Grunt => new RandomScatterStrategy(),
            EnemyType.BetterGrunt => new TargetedStrategy(),
            EnemyType.MidBoss => new AutomaticFireStrategy(),
            EnemyType.FinalBoss => new LaserBeamStrategy(),
            _ => new RandomScatterStrategy()
        };

        return new Enemy(drawer, type, position, velocity, strategy, bulletManager);
    }
}
