using Microsoft.Xna.Framework;
using Core;

namespace Entities.ShootingStrategies;

public class AutomaticFireStrategy : IShootingStrategy
{
    private float _timeSinceLastShot;
    private const float ShotInterval = 0.5f; // Shoot every 0.5 seconds
    private Vector2 _lastPlayerPosition;

    // Add damage property
    private readonly int _damage;

    public AutomaticFireStrategy(int damage = 12)
    {
        _damage = damage;
    }

    public void Update(GameTime gameTime, Vector2 entityPosition, Vector2 playerPosition, BulletManager bulletManager)
    {
        _lastPlayerPosition = playerPosition; // Update last known position

        _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeSinceLastShot >= ShotInterval)
        {
            _timeSinceLastShot = 0;

            // Fire toward last known player position
            Vector2 direction = _lastPlayerPosition - entityPosition;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }
            Vector2 velocity = direction * 300f; // Fast speed

            // Pass damage to the bullet manager
            bulletManager.FireBullet(entityPosition, velocity, _damage);
        }
    }
}