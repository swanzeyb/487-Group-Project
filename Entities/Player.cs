using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;

namespace Entities;

public sealed class Player : IGameEntity
{
    private int _hp = 100;
    public bool IsAlive => _hp > 0;
    public int HP => _hp;
    public Vector2 Position { get; private set; }
    public Rectangle Bounds => new Rectangle((int)Position.X - 10, (int)Position.Y - 10, 20, 20);
    private readonly SimpleDrawer _drawer;
    private readonly InputState _input;

    private bool _slowMode;

    public Player(SimpleDrawer drawer, InputState input)
    {
        _drawer = drawer;
        _input = input;

        Position = new Vector2(
            GameConfig.Playfield.Center.X,
            GameConfig.Playfield.Bottom - 40
        );
    }

    public void Update(GameTime gameTime, Vector2 playerPosition)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _slowMode = _input.Down(Keys.LeftShift);
        float speed = _slowMode ? GameConfig.PlayerSpeedSlow : GameConfig.PlayerSpeedNormal;

        Vector2 move = Vector2.Zero;
        if (_input.Down(Keys.W) || _input.Down(Keys.Up)) move.Y -= 1;
        if (_input.Down(Keys.S) || _input.Down(Keys.Down)) move.Y += 1;
        if (_input.Down(Keys.A) || _input.Down(Keys.Left)) move.X -= 1;
        if (_input.Down(Keys.D) || _input.Down(Keys.Right)) move.X += 1;

        if (move != Vector2.Zero)
            move.Normalize();

        Position += move * speed * dt;

        // confine player to playfield
        Position = new Vector2(
            MathHelper.Clamp(Position.X, GameConfig.Playfield.Left + 10, GameConfig.Playfield.Right - 10),
            MathHelper.Clamp(Position.Y, GameConfig.Playfield.Top + 10, GameConfig.Playfield.Bottom - 10)
        );
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
        if (_hp < 0) _hp = 0;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // player body
        var rect = new Rectangle((int)Position.X - 10, (int)Position.Y - 10, 20, 20);
        _drawer.DrawRect(spriteBatch, rect, Color.White);

        // indicator when slow mode is active
        if (_slowMode)
        {
            var hit = new Rectangle((int)Position.X - 2, (int)Position.Y - 2, 4, 4);
            _drawer.DrawRect(spriteBatch, hit, Color.Blue);
        }
    }
}