using Microsoft.Xna.Framework;
using Core;

namespace Entities.ShootingStrategies;

public class LaserBeamStrategy : IShootingStrategy
{
    private float _timeSinceLastLaser;
    private const float LaserInterval = 5.0f; // Fire laser every 5 seconds

    public void Update(GameTime gameTime, Vector2 entityPosition, Vector2 playerPosition, BulletManager bulletManager)
    {
        _timeSinceLastLaser += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeSinceLastLaser >= LaserInterval)
        {
            _timeSinceLastLaser = 0;

            // Fire laser from entity to player position
            bulletManager.FireLaser(entityPosition, playerPosition);
        }
    }
}