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
    public Rectangle Bounds => new Rectangle((int)Position.X - _size / 2, (int)Position.Y - _size, _size, _size);

    private int _hp;
    private Vector2 _velocity;
    private Color _color;
    private int _size;
    private SimpleDrawer _drawer;
    private IShootingStrategy _shootingStrategy;
    private BulletManager _bulletManager;
    private string _movementPattern;

    public Enemy(SimpleDrawer drawer, EnemyType type, Vector2 startPos, Vector2 velocity, IShootingStrategy shootingStrategy, BulletManager bulletManager, string movementPattern = "linear")
    {
        _drawer = drawer;
        _velocity = velocity;
        Type = type;
        Position = startPos;
        _shootingStrategy = shootingStrategy;
        _bulletManager = bulletManager;
        _movementPattern = movementPattern;

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
                _hp = 100;
                break;
            case EnemyType.FinalBoss:
                _color = Color.DarkRed;
                _size = 120;
                _hp = 300;
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

        // Handle bouncing movement pattern for bosses
        if (_movementPattern == "bounce")
        {
            // Bounce off left and right boundaries of playfield
            if (Position.X - _size / 2 <= GameConfig.Playfield.Left)
            {
                Position = new Vector2(GameConfig.Playfield.Left + _size / 2, Position.Y);
                _velocity.X = Math.Abs(_velocity.X); // Move right
            }
            else if (Position.X + _size / 2 >= GameConfig.Playfield.Right)
            {
                Position = new Vector2(GameConfig.Playfield.Right - _size / 2, Position.Y);
                _velocity.X = -Math.Abs(_velocity.X); // Move left
            }
            
            // Only kill if moving way out of bounds vertically
            if (Position.Y > GameConfig.Playfield.Bottom + 200 ||
                Position.Y < GameConfig.Playfield.Top - 200)
            {
                _hp = 0;
            }
        }
        else
        {
            // Linear movement - kill if out of bounds
            if (Position.Y > GameConfig.Playfield.Bottom + 100 ||
                Position.Y < GameConfig.Playfield.Top - 100 ||
                Position.X < GameConfig.Playfield.Left - 100 ||
                Position.X > GameConfig.Playfield.Right + 100)
            {
                _hp = 0;
            }
        }

        // Update shooting strategy
        _shootingStrategy.Update(gametime, Position, playerPosition, _bulletManager);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var rect = new Rectangle((int)Position.X - _size / 2, (int)Position.Y - _size, _size, _size);
        _drawer.DrawRectOutline(spriteBatch, rect, 2, _color);
    }
}
