using Microsoft.Xna.Framework;
using Core;

namespace Entities.Movement;

public static class PlayerMovementFactory
{
    private static readonly IPlayerMovementStrategy DefaultStrategy = new StandardPlayerMovementStrategy();

    public static IPlayerMovementStrategy CreateDefault() => DefaultStrategy;

    private sealed class StandardPlayerMovementStrategy : IPlayerMovementStrategy
    {
        public Vector2 GetNextPosition(Vector2 currentPosition, Vector2 moveDirection, bool isSlowMode, float dt)
        {
            float speed = isSlowMode ? GameConfig.PlayerSpeedSlow : GameConfig.PlayerSpeedNormal;
            Vector2 next = currentPosition + moveDirection * speed * dt;

            return new Vector2(
                MathHelper.Clamp(next.X, GameConfig.Playfield.Left + 10, GameConfig.Playfield.Right - 10),
                MathHelper.Clamp(next.Y, GameConfig.Playfield.Top + 10, GameConfig.Playfield.Bottom - 10)
            );
        }
    }
}
