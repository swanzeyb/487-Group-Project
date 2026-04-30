using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core;
using Entities.Movement;

namespace Entities;

public enum BulletVisualType
{
    Player,
    Grunt,
    BetterGrunt,
    MidBoss
}

public class Bullet
{
    public bool IsAlive { get; set; } = true;
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public int Damage { get; set; } = 1;
    public bool IsPlayerFired { get; set; } = false;
    public Rectangle Bounds => new Rectangle((int)Position.X - _hitboxSize / 2, (int)Position.Y - _hitboxSize / 2, _hitboxSize, _hitboxSize);

    // Movement runtime state used by strategy implementations.
    internal bool MovementStateInitialized { get; set; }
    internal float MovementElapsedSeconds { get; set; }
    internal float MovementSpeed { get; set; }
    internal Vector2 MovementOrigin { get; set; }
    internal Vector2 MovementDirection { get; set; }
    internal Vector2 MovementPerpendicular { get; set; }

    private readonly SimpleDrawer _drawer;
    private IBulletMovementStrategy _movementStrategy;
    private Texture2D _sprite;
    private int _hitboxSize = 4;
    private Point _drawSize = new Point(4, 4);

    public Bullet(SimpleDrawer drawer, IBulletMovementStrategy movementStrategy = null)
    {
        _drawer = drawer;
        _movementStrategy = movementStrategy ?? BulletMovementFactory.Create(BulletMovementType.Linear);
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _movementStrategy.Update(this, dt);
    }

    public void SetMovementStrategy(IBulletMovementStrategy movementStrategy)
    {
        _movementStrategy = movementStrategy ?? BulletMovementFactory.Create(BulletMovementType.Linear);
        ResetMovementState();
    }

    private void ResetMovementState()
    {
        MovementStateInitialized = false;
        MovementElapsedSeconds = 0f;
        MovementSpeed = 0f;
        MovementOrigin = Vector2.Zero;
        MovementDirection = Vector2.Zero;
        MovementPerpendicular = Vector2.Zero;
    }

    public void ConfigureVisual(Texture2D sprite, int hitboxSize = 4, int drawWidth = 0, int drawHeight = 0)
    {
        _sprite = sprite;
        _hitboxSize = System.Math.Max(2, hitboxSize);

        int width = drawWidth > 0 ? drawWidth : sprite.Width;
        int height = drawHeight > 0 ? drawHeight : sprite.Height;
        _drawSize = new Point(System.Math.Max(1, width), System.Math.Max(1, height));
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_sprite != null)
        {
            var dest = new Rectangle(
                (int)Position.X - _drawSize.X / 2,
                (int)Position.Y - _drawSize.Y / 2,
                _drawSize.X,
                _drawSize.Y);
            spriteBatch.Draw(_sprite, dest, Color.White);
            return;
        }

        _drawer.DrawRect(spriteBatch, Bounds, Color.Red);
    }
}