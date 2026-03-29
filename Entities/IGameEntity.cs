using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entities;

public interface IGameEntity
{
    bool IsAlive {get;}
    void Update(GameTime gameTime, Vector2 playerPosition);
    void Draw(SpriteBatch spriteBatch);
}