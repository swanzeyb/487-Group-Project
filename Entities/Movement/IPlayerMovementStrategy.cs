using Microsoft.Xna.Framework;

namespace Entities.Movement;

public interface IPlayerMovementStrategy
{
    Vector2 GetNextPosition(Vector2 currentPosition, Vector2 moveDirection, bool isSlowMode, float dt);
}
