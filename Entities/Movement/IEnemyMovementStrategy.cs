using Microsoft.Xna.Framework;

namespace Entities.Movement;

public interface IEnemyMovementStrategy
{
    void Update(ref Vector2 position, ref Vector2 velocity, int size, float dt, out bool shouldDespawn);
}
