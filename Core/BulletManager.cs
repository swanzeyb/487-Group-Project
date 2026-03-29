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
    private const int PoolSize = 500;

    public BulletManager(SimpleDrawer drawer)
    {
        _drawer = drawer;
        // Pre-populate the pool
        for (int i = 0; i < PoolSize; i++)
        {
            var bullet = new Bullet(_drawer);
            bullet.IsAlive = false;
            _bullets.Add(bullet);
        }
    }

    public void FireBullet(Vector2 position, Vector2 velocity, int damage = 1, bool isPlayerFired = false)
    {
        var bullet = GetInactiveBullet();
        if (bullet != null)
        {
            bullet.Position = position;
            bullet.Velocity = velocity;
            bullet.Damage = damage;
            bullet.IsPlayerFired = isPlayerFired;
            bullet.IsAlive = true;
        }
    }

    public void FireLaser(Vector2 start, Vector2 end)
    {
        var laser = new Laser(_drawer, start, end);
        _lasers.Add(laser);
    }

    private Bullet? GetInactiveBullet()
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