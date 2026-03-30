using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Entities;
using System.Linq;

namespace Core;

public class BulletManager
{
    private readonly List<Bullet> _bullets = new();
    private readonly List<Laser> _lasers = new();
    private readonly SimpleDrawer _drawer;
    private readonly Texture2D _playerBulletSprite;
    private readonly Texture2D _gruntBulletSprite;
    private readonly Texture2D _betterGruntBulletSprite;
    private readonly Texture2D _midBossBulletSprite;
    private const int PoolSize = 500;

    public BulletManager(
        SimpleDrawer drawer,
        Texture2D playerBulletSprite,
        Texture2D gruntBulletSprite,
        Texture2D betterGruntBulletSprite,
        Texture2D midBossBulletSprite)
    {
        _drawer = drawer;
        _playerBulletSprite = playerBulletSprite;
        _gruntBulletSprite = gruntBulletSprite;
        _betterGruntBulletSprite = betterGruntBulletSprite;
        _midBossBulletSprite = midBossBulletSprite;
        // Pre-populate the pool
        for (int i = 0; i < PoolSize; i++)
        {
            var bullet = new Bullet(_drawer);
            bullet.IsAlive = false;
            _bullets.Add(bullet);
        }
    }

    public void FireBullet(
        Vector2 position,
        Vector2 velocity,
        int damage = 1,
        bool isPlayerFired = false,
        BulletVisualType visualType = BulletVisualType.Grunt)
    {
        var bullet = GetInactiveBullet();
        if (bullet != null)
        {
            bullet.Position = position;
            bullet.Velocity = velocity;
            bullet.Damage = damage;
            bullet.IsPlayerFired = isPlayerFired;
            Point drawSize = GetDrawSize(visualType);
            bullet.ConfigureVisual(GetBulletSprite(visualType), GetHitboxSize(visualType), drawSize.X, drawSize.Y);
            bullet.IsAlive = true;
        }
    }

    public void FireLaser(Vector2 start, Vector2 end)
    {
        var laser = new Laser(_drawer, start, end);
        _lasers.Add(laser);
    }

    private Texture2D GetBulletSprite(BulletVisualType visualType) => visualType switch
    {
        BulletVisualType.Player => _playerBulletSprite,
        BulletVisualType.Grunt => _gruntBulletSprite,
        BulletVisualType.BetterGrunt => _betterGruntBulletSprite,
        BulletVisualType.MidBoss => _midBossBulletSprite,
        _ => _gruntBulletSprite
    };

    private static int GetHitboxSize(BulletVisualType visualType) => visualType switch
    {
        BulletVisualType.Player => 5,
        BulletVisualType.Grunt => 5,
        BulletVisualType.BetterGrunt => 6,
        BulletVisualType.MidBoss => 6,
        _ => 5
    };

    private static Point GetDrawSize(BulletVisualType visualType) => visualType switch
    {
        // Keep projectiles visually consistent regardless of source texture dimensions.
        BulletVisualType.Player => new Point(18, 9),
        BulletVisualType.Grunt => new Point(12, 12),
        BulletVisualType.BetterGrunt => new Point(12, 12),
        BulletVisualType.MidBoss => new Point(14, 14),
        _ => new Point(12, 12)
    };

    private Bullet GetInactiveBullet()
    {
        foreach (var bullet in _bullets)
        {
            if (!bullet.IsAlive)
            {
                return bullet;
            }
        }
        return null; // Pool exhausted, could expand pool or return null
    }

    public void Update(GameTime gameTime)
    {
        foreach (var bullet in _bullets)
        {
            if (bullet.IsAlive)
            {
                bullet.Update(gameTime);
            }
        }

        for (int i = _lasers.Count - 1; i >= 0; i--)
        {
            _lasers[i].Update(gameTime);
            if (!_lasers[i].IsAlive)
            {
                _lasers.RemoveAt(i);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var bullet in _bullets)
        {
            if (bullet.IsAlive)
            {
                bullet.Draw(spriteBatch);
            }
        }

        foreach (var laser in _lasers)
        {
            laser.Draw(spriteBatch);
        }
    }

    public IEnumerable<Bullet> ActiveBullets => _bullets.Where(b => b.IsAlive);
    public IEnumerable<Laser> ActiveLasers => _lasers.Where(l => l.IsAlive);
}