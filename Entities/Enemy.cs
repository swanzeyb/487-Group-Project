using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core;

namespace Entities;

public enum EnemyType
{
    Grunt,
    BetterGrunt,
    MidBoss,
    FinalBoss
}

public class Enemy:IGameEntity
{
    public bool IsAlive { get; private set; } = true;
    public Vector2 Position { get; private set; }
    public EnemyType Type { get; private set; }

    private Vector2 _velocity;
    private Color _color;
    private int _size;
    private SimpleDrawer _drawer;

    public Enemy(SimpleDrawer drawer, EnemyType type, Vector2 startPos, Vector2 velocity)
    {
        _drawer = drawer;
        _velocity = velocity;
        Type = type;
        Position = startPos;

        // Switch case to give the enemy their color and size based on their type.
        switch (type)
        {
            case EnemyType.Grunt:
                _color = Color.Blue;
                _size = 16;
                break;
            case EnemyType.BetterGrunt:
                _color = Color.Orange;
                _size = 24;
                break;
            case EnemyType.MidBoss:
                _color = Color.Purple;
                _size = 60;
                break;
            case EnemyType.FinalBoss:
                _color = Color.DarkRed;
                _size = 120;
                break;
        }
    }

    public void Update(GameTime gametime)
    {
        float dt = (float)gametime.ElapsedGameTime.TotalSeconds;
        Position += _velocity * dt;

        // Enemy will despawn if they move outside the playfield.
        if (Position.Y > GameConfig.ScreenHeight + 100 ||
            Position.Y < -100 ||
            Position.X < -100 ||
            Position.X > GameConfig.ScreenWidth + 100)
        {
            IsAlive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var rect = new Rectangle((int)Position.X - _size / 2, (int)Position.Y - _size, _size, _size);
        _drawer.DrawRectOutline(spriteBatch, rect, 2, _color);
    }
}
