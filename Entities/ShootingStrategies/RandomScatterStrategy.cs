using System;
using Microsoft.Xna.Framework;
using Core;

namespace Entities.ShootingStrategies;

public class RandomScatterStrategy : IShootingStrategy
{
    private readonly Random _random = new();
    private float _timeSinceLastShot;
    private const float ShotInterval = 2.0f; // Shoot every 2 seconds
    private readonly int _damage;

    public RandomScatterStrategy(int damage = 1)
    {
        _damage = damage;
    }

    public void Update(GameTime gameTime, Vector2 entityPosition, Vector2 playerPosition, BulletManager bulletManager)
    {
        _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeSinceLastShot >= ShotInterval)
        {
            _timeSinceLastShot = 0;

            // Fire three bullets in random directions
            for (int i = 0; i < 3; i++)
            {
                float angle = (float)(_random.NextDouble() * Math.PI * 2);
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 velocity = direction * 200f; // Speed
                bulletManager.FireBullet(entityPosition, velocity, damage: _damage);
            }
        }
    }
}