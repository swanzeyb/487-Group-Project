using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core;

namespace Entities;

public class Laser
{
    public bool IsAlive { get; set; } = true;
    public Rectangle Bounds { get; private set; }
    public bool IsWarning { get; private set; }
    public bool IsActive { get; private set; }

    private readonly SimpleDrawer _drawer;
    private float _timer;
    private const float WarningTime = 1.0f;
    private readonly Vector2 _startPosition;
    private readonly Vector2 _endPosition;

    public Laser(SimpleDrawer drawer, Vector2 start, Vector2 end)
    {
        _drawer = drawer;
        _startPosition = start;
        _endPosition = end;
        Bounds = new Rectangle((int)start.X, (int)start.Y, (int)(end - start).Length(), 50);
        IsWarning = true;
        IsActive = false;
    }

    public void Update(GameTime gameTime)
    {
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer >= WarningTime)
        {
            IsWarning = false;
            IsActive = true;
        }

        // Laser persists for a short time after activation, set to 0.5 seconds
        if (_timer >= WarningTime + 0.5f)
        {
            IsAlive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color color = IsWarning ? Color.Yellow : Color.Red;
        _drawer.DrawRect(spriteBatch, Bounds, color);
    }
}