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
    public bool IsAlive => _hp > 0;
    public Vector2 Position { get; private set; }
    public EnemyType Type { get; private set; }
    public int HP => _hp;

    private int _hp;
    private Vector2 _velocity;
    private Color _color;
    private int _size;
    private SimpleDrawer _drawer;
    private IShootingStrategy _shootingStrategy;
    private BulletManager _bulletManager;

    public Enemy(SimpleDrawer drawer, EnemyType type, Vector2 startPos, Vector2 velocity, IShootingStrategy shootingStrategy, BulletManager bulletManager)
    {
        _drawer = drawer;
        _velocity = velocity;
        Type = type;
        Position = startPos;
        _shootingStrategy = shootingStrategy;
        _bulletManager = bulletManager;

        // Switch case to give the enemy their color, size, and HP based on their type.
        switch (type)
        {
            case EnemyType.Grunt:
                _color = Color.Blue;
                _size = 16;
                _hp = 5;
                break;
            case EnemyType.BetterGrunt:
                _color = Color.Orange;
                _size = 24;
                _hp = 10;
                break;
            case EnemyType.MidBoss:
                _color = Color.Purple;
                _size = 60;
                _hp = 50;
                break;
            case EnemyType.FinalBoss:
                _color = Color.DarkRed;
                _size = 120;
                _hp = 100;
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
        if (_hp < 0) _hp = 0;
    }

    public void Update(GameTime gametime, Vector2 playerPosition)
    {
        float dt = (float)gametime.ElapsedGameTime.TotalSeconds;
        Position += _velocity * dt;

        // Update shooting strategy
        _shootingStrategy.Update(gametime, Position, playerPosition, _bulletManager);

        // Enemies despawns if they move outside the playfield bounds.
        if (Position.Y > GameConfig.Playfield.Bottom + 100 ||
            Position.Y < GameConfig.Playfield.Top - 100 ||
            Position.X < GameConfig.Playfield.Left - 100 ||
            Position.X > GameConfig.Playfield.Right + 100)
        {
            _hp = 0; // Kill if out of bounds
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var rect = new Rectangle((int)Position.X - _size / 2, (int)Position.Y - _size, _size, _size);
        _drawer.DrawRectOutline(spriteBatch, rect, 2, _color);
    }
}
