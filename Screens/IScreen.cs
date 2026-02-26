using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core;

namespace Screens;

public interface IScreen
{
    void OnEnter();
    void OnExit();
    void Update(GameTime gameTime, InputState input);
    void Draw(SpriteBatch spriteBatch, SimpleDrawer drawer);
}
