using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Entities.Movement;
using Entities.ShootingStrategies;

namespace Entities;

/// <summary>
/// Centralised factory for creating Enemy instances.
/// Teammates adding HP, bullet patterns, etc. only need to modify this one place.
/// </summary>
public static class EnemyFactory
{
    public static Enemy Create(SimpleDrawer drawer, EnemyType type, Vector2 position, Vector2 velocity, BulletManager bulletManager, string movementPattern = "linear", Texture2D sprite = null)
    {
        IShootingStrategy strategy = type switch
        {
            EnemyType.Grunt => new RandomScatterStrategy(damage: 5),
            EnemyType.BetterGrunt => new TargetedStrategy(damage: 5),
            EnemyType.MidBoss => new AutomaticFireStrategy(damage: 10),
            EnemyType.FinalBoss => new LaserBeamStrategy(),
            _ => new RandomScatterStrategy()
        };

        IEnemyMovementStrategy enemyMovement = EnemyMovementFactory.Create(movementPattern);

        return new Enemy(drawer, type, position, velocity, strategy, bulletManager, movementPattern, sprite, enemyMovement);
    }
}
