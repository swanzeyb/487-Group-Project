namespace Entities.Movement;

public interface IBulletMovementStrategy
{
    void Update(Bullet bullet, float dt);
}
