using System;
using System.Collections.Generic;
using Entities;
using Microsoft.Xna.Framework;

namespace Core;

public sealed class CollisionManager
{
    public void DetectPlayerBulletEnemyCollisions(
        IEnumerable<Bullet> bullets,
        IEnumerable<Enemy> enemies,
        Action<Bullet, Enemy> onCollision)
    {
        foreach (var bullet in bullets)
        {
            if (!bullet.IsPlayerFired || !bullet.IsAlive)
            {
                continue;
            }

            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive)
                {
                    continue;
                }

                if (enemy.Bounds.Intersects(bullet.Bounds))
                {
                    onCollision(bullet, enemy);
                    break;
                }
            }
        }
    }

    public void DetectEnemyBulletPlayerCollisions(
        IEnumerable<Bullet> bullets,
        Player player,
        Action<Bullet> onCollision)
    {
        foreach (var bullet in bullets)
        {
            if (bullet.IsPlayerFired || !bullet.IsAlive)
            {
                continue;
            }

            if (player.Bounds.Intersects(bullet.Bounds))
            {
                onCollision(bullet);
            }
        }
    }

    public void DetectLaserPlayerCollisions(
        IEnumerable<Laser> lasers,
        Rectangle playerBounds,
        Action<Laser> onCollision)
    {
        foreach (var laser in lasers)
        {
            if (!laser.IsActive)
            {
                continue;
            }

            if (LaserIntersectsPlayer(laser, playerBounds))
            {
                onCollision(laser);
            }
        }
    }

    public void DetectProjectilePlayerCollisions<TProjectile>(
        IEnumerable<TProjectile> projectiles,
        Rectangle playerBounds,
        Func<TProjectile, Rectangle> boundsSelector,
        Action<TProjectile> onCollision)
    {
        foreach (var projectile in projectiles)
        {
            if (playerBounds.Intersects(boundsSelector(projectile)))
            {
                onCollision(projectile);
            }
        }
    }

    private static bool LaserIntersectsPlayer(Laser laser, Rectangle playerBounds)
    {
        Vector2 playerCenter = playerBounds.Center.ToVector2();
        float playerRadius = Math.Min(playerBounds.Width, playerBounds.Height) / 2f;
        Vector2 start = laser.StartPosition;
        Vector2 end = laser.EndPosition;
        Vector2 segment = end - start;
        float segmentLengthSq = segment.LengthSquared();

        if (segmentLengthSq < 0.0001f)
        {
            return Vector2.Distance(playerCenter, start) <= Laser.BeamThickness / 2 + playerRadius;
        }

        float t = Vector2.Dot(playerCenter - start, segment) / segmentLengthSq;
        t = MathHelper.Clamp(t, 0f, 1f);
        Vector2 closest = start + segment * t;
        float distance = Vector2.Distance(playerCenter, closest);
        return distance <= Laser.BeamThickness / 2 + playerRadius;
    }
}
