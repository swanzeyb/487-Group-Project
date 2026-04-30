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
            "bounce" => new BounceEnemyMovementStrategy(),
            "sinusoidal" => new SinusoidalEnemyMovementStrategy(),
            _ => new LinearEnemyMovementStrategy()
        };
    }

    private sealed class LinearEnemyMovementStrategy : IEnemyMovementStrategy
    {
        public void Update(ref Vector2 position, ref Vector2 velocity, int size, float dt, out bool shouldDespawn)
        {
            position += velocity * dt;

            shouldDespawn = position.Y > GameConfig.Playfield.Bottom + 100 ||
                            position.Y < GameConfig.Playfield.Top - 100 ||
                            position.X < GameConfig.Playfield.Left - 100 ||
                            position.X > GameConfig.Playfield.Right + 100;
        }
    }

    private sealed class BounceEnemyMovementStrategy : IEnemyMovementStrategy
    {
        public void Update(ref Vector2 position, ref Vector2 velocity, int size, float dt, out bool shouldDespawn)
        {
            position += velocity * dt;

            if (position.X - size / 2 <= GameConfig.Playfield.Left)
            {
                position = new Vector2(GameConfig.Playfield.Left + size / 2f, position.Y);
                velocity.X = (float)Math.Abs(velocity.X);
            }
            else if (position.X + size / 2 >= GameConfig.Playfield.Right)
            {
                position = new Vector2(GameConfig.Playfield.Right - size / 2f, position.Y);
                velocity.X = -(float)Math.Abs(velocity.X);
            }

            shouldDespawn = position.Y > GameConfig.Playfield.Bottom + 200 ||
                            position.Y < GameConfig.Playfield.Top - 200;
        }
    }

    private sealed class SinusoidalEnemyMovementStrategy : IEnemyMovementStrategy
    {
        private const float AngularFrequency = 2.8f;
        private const float LateralAmplitude = 45f;

        private bool _initialized;
        private float _elapsedSeconds;
        private float _speed;
        private Vector2 _origin;
        private Vector2 _direction;
        private Vector2 _perpendicular;

        public void Update(ref Vector2 position, ref Vector2 velocity, int size, float dt, out bool shouldDespawn)
        {
            if (!_initialized)
            {
                _initialized = true;
                _elapsedSeconds = 0f;
                _origin = position;

                _speed = velocity.Length();
                _direction = _speed > 0.0001f ? Vector2.Normalize(velocity) : new Vector2(0f, 1f);
                _perpendicular = new Vector2(-_direction.Y, _direction.X);
            }

            _elapsedSeconds += dt;

            float forwardDistance = _speed * _elapsedSeconds;
            float lateralOffset = (float)Math.Sin(_elapsedSeconds * AngularFrequency) * LateralAmplitude;

            position = _origin + _direction * forwardDistance + _perpendicular * lateralOffset;

            shouldDespawn = position.Y > GameConfig.Playfield.Bottom + 100 ||
                            position.Y < GameConfig.Playfield.Top - 100 ||
                            position.X < GameConfig.Playfield.Left - 100 ||
                            position.X > GameConfig.Playfield.Right + 100;
        }
    }
}
