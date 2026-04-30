using System;
using Core;
using Microsoft.Xna.Framework;

namespace Entities.Movement;

public enum BulletMovementType
{
    Linear,
    Sinusoidal
}

public static class BulletMovementFactory
{
    private static readonly IBulletMovementStrategy LinearStrategy = new LinearBulletMovementStrategy();
    private static readonly IBulletMovementStrategy SinusoidalStrategy = new SinusoidalBulletMovementStrategy();

    public static IBulletMovementStrategy Create(BulletMovementType type) => type switch
    {
        BulletMovementType.Linear => LinearStrategy,
        BulletMovementType.Sinusoidal => SinusoidalStrategy,
        _ => LinearStrategy
    };

    private sealed class LinearBulletMovementStrategy : IBulletMovementStrategy
    {
        public void Update(Bullet bullet, float dt)
        {
            bullet.Position += bullet.Velocity * dt;

            if (IsOutOfBounds(bullet.Position))
            {
                bullet.IsAlive = false;
            }
        }
    }

    private sealed class SinusoidalBulletMovementStrategy : IBulletMovementStrategy
    {
        private const float AngularFrequency = 8f;
        private const float LateralAmplitude = 22f;

        public void Update(Bullet bullet, float dt)
        {
            if (!bullet.MovementStateInitialized)
            {
                Vector2 direction = bullet.Velocity;
                if (direction == Vector2.Zero)
                {
                    direction = new Vector2(0f, -1f);
                }
                direction.Normalize();

                bullet.MovementStateInitialized = true;
                bullet.MovementElapsedSeconds = 0f;
                bullet.MovementSpeed = bullet.Velocity.Length();
                bullet.MovementOrigin = bullet.Position;
                bullet.MovementDirection = direction;
                bullet.MovementPerpendicular = new Vector2(-direction.Y, direction.X);
            }

            bullet.MovementElapsedSeconds += dt;

            float forwardDistance = bullet.MovementSpeed * bullet.MovementElapsedSeconds;
            float lateralOffset = (float)Math.Sin(bullet.MovementElapsedSeconds * AngularFrequency) * LateralAmplitude;
            bullet.Position = bullet.MovementOrigin
                + bullet.MovementDirection * forwardDistance
                + bullet.MovementPerpendicular * lateralOffset;

            if (IsOutOfBounds(bullet.Position))
            {
                bullet.IsAlive = false;
            }
        }
    }

    private static bool IsOutOfBounds(Vector2 position)
    {
        return position.Y > GameConfig.Playfield.Bottom + 100 ||
               position.Y < GameConfig.Playfield.Top - 100 ||
               position.X < GameConfig.Playfield.Left - 100 ||
               position.X > GameConfig.Playfield.Right + 100;
    }
}
