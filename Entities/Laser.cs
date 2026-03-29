using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Core;

namespace Entities;

public class Laser
{
    public const float BeamThickness = 15f;

    public bool IsAlive { get; set; } = true;
    public Rectangle Bounds { get; private set; }
    public bool IsWarning { get; private set; }
    public bool IsActive { get; private set; }

    // expose endpoints for collision logic
    public Vector2 StartPosition => _startPosition;
    public Vector2 EndPosition => _endPosition;

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
        // build an axis-aligned bounding box for the laser (used in collision checks)
        float minX = Math.Min(start.X, end.X);
        float minY = Math.Min(start.Y, end.Y);
        float width = Math.Abs(end.X - start.X);
        float height = Math.Abs(end.Y - start.Y);
        // give the bounding box some thickness so it actually intersects the player
        float thickness = 15f;
        Bounds = new Rectangle((int)minX, (int)minY, (int)Math.Max(width, thickness), (int)Math.Max(height, thickness));
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
        // draw a beam from the start to the end position, rotated automatically by the drawer
        _drawer.DrawLine(spriteBatch, _startPosition, _endPosition, color, 15f);
    }
}