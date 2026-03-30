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
        if (IsWarning)
        {
            float blink = 0.35f + 0.65f * (float)Math.Abs(Math.Sin(_timer * 18f));
            var warningGlow = new Color(255, 200, 70, (int)(90f * blink));
            var warningCore = new Color(255, 245, 180, (int)(180f * blink));

            _drawer.DrawLine(spriteBatch, _startPosition, _endPosition, warningGlow, BeamThickness * 0.42f);
            _drawer.DrawLine(spriteBatch, _startPosition, _endPosition, warningCore, 3f);
            _drawer.DrawCircle(spriteBatch, _startPosition, 8f, warningCore, segments: 20, thickness: 2);
            _drawer.DrawCircle(spriteBatch, _endPosition, 8f, warningCore, segments: 20, thickness: 2);
            return;
        }

        float pulse = 0.88f + 0.12f * (float)Math.Sin(_timer * 24f);
        var beamGlow = new Color(255, 70, 70, 120);
        var beamMid = new Color(255, 145, 90, 170);
        var beamCore = new Color(255, 245, 235, 230);

        _drawer.DrawLine(spriteBatch, _startPosition, _endPosition, beamGlow, BeamThickness * 1.55f * pulse);
        _drawer.DrawLine(spriteBatch, _startPosition, _endPosition, beamMid, BeamThickness * 0.95f);
        _drawer.DrawLine(spriteBatch, _startPosition, _endPosition, beamCore, BeamThickness * 0.35f);

        _drawer.DrawCircle(spriteBatch, _startPosition, 11f, beamMid, segments: 24, thickness: 3);
        _drawer.DrawCircle(spriteBatch, _endPosition, 11f, beamMid, segments: 24, thickness: 3);
    }
}