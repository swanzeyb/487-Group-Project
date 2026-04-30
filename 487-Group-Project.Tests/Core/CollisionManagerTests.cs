using Core;
using Entities;
using Microsoft.Xna.Framework;

namespace _487_Group_Project.Tests.Core;

[TestFixture]
public class CollisionManagerTests
{
    [Test]
    public void DetectPlayerBulletEnemyCollisions_DelegatesCollisionToCallback()
    {
        var manager = new CollisionManager();
        var bullet = new Bullet(null!)
        {
            Position = new Vector2(100, 80),
            Velocity = Vector2.Zero,
            IsAlive = true,
            IsPlayerFired = true
        };

        var enemy = new Enemy(null!, EnemyType.Grunt, new Vector2(100, 100), Vector2.Zero, new NoOpShootingStrategy(), null!);

        int callbacks = 0;
        manager.DetectPlayerBulletEnemyCollisions(new[] { bullet }, new[] { enemy }, (_, _) => callbacks++);

        Assert.That(callbacks, Is.EqualTo(1));
        Assert.That(bullet.IsAlive, Is.True);
        Assert.That(enemy.HP, Is.EqualTo(5));
    }

    [Test]
    public void DetectEnemyBulletPlayerCollisions_DelegatesCollisionToCallback()
    {
        var manager = new CollisionManager();
        var input = new InputState();
        var player = new Player(null!, input, null!);
        var bullet = new Bullet(null!)
        {
            Position = player.Position,
            Velocity = Vector2.Zero,
            IsAlive = true,
            IsPlayerFired = false
        };

        int callbacks = 0;
        manager.DetectEnemyBulletPlayerCollisions(new[] { bullet }, player, _ => callbacks++);

        Assert.That(callbacks, Is.EqualTo(1));
        Assert.That(player.HP, Is.EqualTo(100));
    }

    [Test]
    public void DetectLaserPlayerCollisions_DelegatesCollisionToCallback()
    {
        var manager = new CollisionManager();
        var input = new InputState();
        var player = new Player(null!, input, null!);

        var laser = new Laser(null!, new Vector2(player.Position.X - 50, player.Position.Y), new Vector2(player.Position.X + 50, player.Position.Y));
        laser.Update(new GameTime(TimeSpan.FromSeconds(1.1), TimeSpan.FromSeconds(1.1)));

        int callbacks = 0;
        manager.DetectLaserPlayerCollisions(new[] { laser }, player.Bounds, _ => callbacks++);

        Assert.That(callbacks, Is.EqualTo(1));
    }

    [Test]
    public void DetectProjectilePlayerCollisions_DelegatesCollisionToCallback()
    {
        var manager = new CollisionManager();
        var input = new InputState();
        var player = new Player(null!, input, null!);

        var projectiles = new[]
        {
            new Rectangle(player.Bounds.X, player.Bounds.Y, player.Bounds.Width, player.Bounds.Height)
        };

        int callbacks = 0;
        manager.DetectProjectilePlayerCollisions(projectiles, player.Bounds, r => r, _ => callbacks++);

        Assert.That(callbacks, Is.EqualTo(1));
    }

    private sealed class NoOpShootingStrategy : IShootingStrategy
    {
        public void Update(GameTime gameTime, Vector2 enemyPosition, Vector2 playerPosition, BulletManager bulletManager)
        {
        }
    }
}
