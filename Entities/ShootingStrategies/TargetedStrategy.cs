using System;
using Microsoft.Xna.Framework;
using Core;

namespace Entities.ShootingStrategies;

public class TargetedStrategy : IShootingStrategy
{
    private float _timeSinceLastShot;
    private const float ShotInterval = 1.5f; // Shoot every 1.5 seconds
    private readonly int _damage;

    public TargetedStrategy(int damage = 1)
    {
        _damage = damage;
    }

    public void Update(GameTime gameTime, Vector2 entityPosition, Vector2 playerPosition, BulletManager bulletManager)
    {
        _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeSinceLastShot >= ShotInterval)
        {
            _timeSinceLastShot = 0;

            // Calculate direction to player
            Vector2 direction = playerPosition - entityPosition;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }
            Vector2 velocity = direction * 250f; // Speed
            bulletManager.FireBullet(entityPosition, velocity, damage: _damage);
        }
    }
}