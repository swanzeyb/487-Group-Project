using Microsoft.Xna.Framework;
using Core;

namespace Entities;

public interface IShootingStrategy
{
    void Update(GameTime gameTime, Vector2 entityPosition, Vector2 playerPosition, BulletManager bulletManager);
}