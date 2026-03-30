using Microsoft.Xna.Framework;
using System;
using Core;

namespace Entities.ShootingStrategies;

public class LaserBeamStrategy : IShootingStrategy
{
    private float _timeSinceLastLaser;
    private const float LaserInterval = 1.5f; // Fire laser faster in final boss phase
    private readonly int _damage;

    public LaserBeamStrategy(int damage = 0)
    {
        // Lasers do 0 damage directly, instant death on contact instead
        _damage = damage;
    }

    public void Update(GameTime gameTime, Vector2 entityPosition, Vector2 playerPosition, BulletManager bulletManager)
    {
        _timeSinceLastLaser += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeSinceLastLaser >= LaserInterval)
        {
            _timeSinceLastLaser = 0;

            // determine beam endpoint so that it extends through the player and hits the playfield boundary
            Vector2 direction = playerPosition - entityPosition;
            if (direction == Vector2.Zero)
            {
                // avoid zero-length by defaulting straight down
                direction = new Vector2(0, 1);
            }
            direction.Normalize();

            // compute intersection with playfield edges; t is the distance along direction until we hit a border
            float t = float.MaxValue;
            var pf = GameConfig.Playfield;
            // check vertical boundaries
            if (direction.X > 0)
            {
                float tx = (pf.Right - entityPosition.X) / direction.X;
                if (tx > 0 && tx < t) t = tx;
            }
            else if (direction.X < 0)
            {
                float tx = (pf.Left - entityPosition.X) / direction.X;
                if (tx > 0 && tx < t) t = tx;
            }
            if (direction.Y > 0)
            {
                float ty = (pf.Bottom - entityPosition.Y) / direction.Y;
                if (ty > 0 && ty < t) t = ty;
            }
            else if (direction.Y < 0)
            {
                float ty = (pf.Top - entityPosition.Y) / direction.Y;
                if (ty > 0 && ty < t) t = ty;
            }
            
            if (t == float.MaxValue)
            {
                // should not happen, but fallback to a long beam
                t = Math.Max(pf.Width, pf.Height) * 2;
            }

            Vector2 endPoint = entityPosition + direction * t;
            bulletManager.FireLaser(entityPosition, endPoint);
        }
    }
}