using System;
using Core;
using Microsoft.Xna.Framework;

namespace Entities.Movement;

public static class EnemyMovementFactory
{
    public static IEnemyMovementStrategy Create(string movementPattern)
    {
        string normalized = movementPattern?.Trim().ToLowerInvariant() ?? "linear";
        return normalized switch
        {
            "sinusoidal" => new SinusoidalEnemyMovementStrategy(),
            "circular"   => new CircularEnemyMovementStrategy(),
            _            => new LinearEnemyMovementStrategy()
        };
    }

    private sealed class LinearEnemyMovementStrategy : IEnemyMovementStrategy
    {
        public void Update(ref Vector2 position, ref Vector2 velocity, int size, float dt, out bool shouldDespawn)
        {
            position += velocity * dt;
            ApplyHorizontalBounce(ref position, ref velocity, size);

            shouldDespawn = position.Y > GameConfig.Playfield.Bottom + 100 || position.Y < GameConfig.Playfield.Top - 100;
        }
    }

    private sealed class SinusoidalEnemyMovementStrategy : IEnemyMovementStrategy
    {
        private const float AngularFrequency = 2.8f;
        private const float LateralAmplitude = 45f;

        private bool _initialized;
        private float _phase;
        private float _previousLateralOffset;

        public void Update(ref Vector2 position, ref Vector2 velocity, int size, float dt, out bool shouldDespawn)
        {
            if (!_initialized)
            {
                _initialized = true;
                _phase = 0f;
                _previousLateralOffset = 0f;
            }

            Vector2 forward = velocity;
            if (forward.LengthSquared() < 0.0001f)
            {
                forward = new Vector2(0f, 1f);
            }
            forward.Normalize();
            Vector2 perpendicular = new Vector2(-forward.Y, forward.X);

            _phase += dt * AngularFrequency;
            float lateralOffset = (float)Math.Sin(_phase) * LateralAmplitude;
            float lateralDelta = lateralOffset - _previousLateralOffset;
            _previousLateralOffset = lateralOffset;

            position += velocity * dt + perpendicular * lateralDelta;

            bool bounced = ApplyHorizontalBounce(ref position, ref velocity, size);
            if (bounced)
            {
                _phase = 0f;
                _previousLateralOffset = 0f;
            }

            shouldDespawn = position.Y > GameConfig.Playfield.Bottom + 100 ||
                            position.Y < GameConfig.Playfield.Top - 100;
        }
    }

    private sealed class CircularEnemyMovementStrategy : IEnemyMovementStrategy
    {
        private const float Radius = 80f;
        private const float AngularSpeed = 1.5f; // radians/sec (~4s per revolution)

        private bool _initialized;
        private Vector2 _center;
        private float _angle;

        public void Update(ref Vector2 position, ref Vector2 velocity, int size, float dt, out bool shouldDespawn)
        {
            if (!_initialized)
            {
                _initialized = true;
                _center = position;
                _angle = -(float)Math.PI / 2f; // start at top of circle
            }

            _angle += AngularSpeed * dt;

            float halfSize = size / 2f;
            float clampedCenterX = MathHelper.Clamp(
                _center.X,
                GameConfig.Playfield.Left  + Radius + halfSize,
                GameConfig.Playfield.Right - Radius - halfSize);
            float clampedCenterY = MathHelper.Clamp(
                _center.Y,
                GameConfig.Playfield.Top    + Radius + halfSize,
                GameConfig.Playfield.Bottom - Radius - halfSize);

            position = new Vector2(
                clampedCenterX + Radius * (float)Math.Cos(_angle),
                clampedCenterY + Radius * (float)Math.Sin(_angle));

            shouldDespawn = false;
        }
    }

    private static bool ApplyHorizontalBounce(ref Vector2 position, ref Vector2 velocity, int size)
    {
        float halfSize = size / 2f;
        if (position.X - halfSize <= GameConfig.Playfield.Left)
        {
            position = new Vector2(GameConfig.Playfield.Left + halfSize, position.Y);
            velocity.X = (float)Math.Abs(velocity.X);
            return true;
        }

        if (position.X + halfSize >= GameConfig.Playfield.Right)
        {
            position = new Vector2(GameConfig.Playfield.Right - halfSize, position.Y);
            velocity.X = -(float)Math.Abs(velocity.X);
            return true;
        }

        return false;
    }
}
