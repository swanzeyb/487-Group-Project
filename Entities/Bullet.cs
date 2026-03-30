using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core;

namespace Entities;

public class Bullet
{
    public bool IsAlive { get; set; } = true;
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public int Damage { get; set; } = 1;
    public bool IsPlayerFired { get; set; } = false;
    public Rectangle Bounds => new Rectangle((int)Position.X - 2, (int)Position.Y - 2, 4, 4);

    private readonly SimpleDrawer _drawer;

    public Bullet(SimpleDrawer drawer)
    {
        _drawer = drawer;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;

        // Deactivate if out of bounds
        if (Position.Y > GameConfig.Playfield.Bottom + 100 ||
            Position.Y < GameConfig.Playfield.Top - 100 ||
            Position.X < GameConfig.Playfield.Left - 100 ||
            Position.X > GameConfig.Playfield.Right + 100)
        {
            IsAlive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _drawer.DrawRect(spriteBatch, Bounds, Color.Red);
    }
}