using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public sealed class SimpleDrawer
{
    private readonly Texture2D _pixel;

    public SimpleDrawer(GraphicsDevice gd)
    {
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public Texture2D Pixel => _pixel;

    public void DrawRect(SpriteBatch sb, Rectangle rect, Color color)
        => sb.Draw(_pixel, rect, color);

    public void DrawRectOutline(SpriteBatch sb, Rectangle rect, int thickness, Color color)
    {
        // top
        DrawRect(sb, new Rectangle(rect.Left, rect.Top, rect.Width, thickness), color);
        // bottom
        DrawRect(sb, new Rectangle(rect.Left, rect.Bottom - thickness, rect.Width, thickness), color);
        // left
        DrawRect(sb, new Rectangle(rect.Left, rect.Top, thickness, rect.Height), color);
        // right
        DrawRect(sb, new Rectangle(rect.Right - thickness, rect.Top, thickness, rect.Height), color);
    }

    public void DrawCircle(SpriteBatch sb, Vector2 center, float radius, Color color, int segments = 32, int thickness = 2)
    {
        // Cheap circle outline using line segments
        var prev = center + new Vector2(radius, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float a = MathHelper.TwoPi * i / segments;
            var next = center + new Vector2((float)System.Math.Cos(a), (float)System.Math.Sin(a)) * radius;
            DrawLine(sb, prev, next, color, thickness);
            prev = next;
        }
    }

    public void DrawLine(SpriteBatch sb, Vector2 a, Vector2 b, Color color, float thickness)
    {
        var d = b - a;
        float len = d.Length();
        if (len <= 0.0001f) return;
        float rot = (float)System.Math.Atan2(d.Y, d.X);
        sb.Draw(_pixel, a, null, color, rot, Vector2.Zero, new Vector2(len, thickness), SpriteEffects.None, 0f);
    }
    

}