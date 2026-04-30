using Core;
using Entities;
using Entities.Movement;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace _487_Group_Project.Tests.Entities;

[TestFixture]
public class MovementFactoryTests
{
    [Test]
    public void PlayerMovementFactory_DefaultStrategy_UsesNormalSpeed()
    {
        var strategy = PlayerMovementFactory.CreateDefault();
        Vector2 start = new Vector2(200, 200);

        Vector2 next = strategy.GetNextPosition(start, new Vector2(1, 0), isSlowMode: false, dt: 1f);

        Assert.That(next.X, Is.EqualTo(start.X + GameConfig.PlayerSpeedNormal));
        Assert.That(next.Y, Is.EqualTo(start.Y));
    }

    [Test]
    public void PlayerMovementFactory_DefaultStrategy_UsesSlowSpeed()
    {
        var strategy = PlayerMovementFactory.CreateDefault();
        Vector2 start = new Vector2(200, 200);

        Vector2 next = strategy.GetNextPosition(start, new Vector2(1, 0), isSlowMode: true, dt: 1f);

        Assert.That(next.X, Is.EqualTo(start.X + GameConfig.PlayerSpeedSlow));
        Assert.That(next.Y, Is.EqualTo(start.Y));
    }

    [Test]
    public void PlayerMovementFactory_DefaultStrategy_ClampsToPlayfield()
    {
        var strategy = PlayerMovementFactory.CreateDefault();
        Vector2 start = new Vector2(GameConfig.Playfield.Left + 10, GameConfig.Playfield.Top + 10);

        Vector2 next = strategy.GetNextPosition(start, new Vector2(-1, -1), isSlowMode: false, dt: 1f);

        Assert.That(next.X, Is.EqualTo(GameConfig.Playfield.Left + 10));
        Assert.That(next.Y, Is.EqualTo(GameConfig.Playfield.Top + 10));
    }

    [Test]
    public void BulletMovementFactory_LinearStrategy_UpdatesPositionFromVelocity()
    {
        var bullet = new Bullet(null!, BulletMovementFactory.Create(BulletMovementType.Linear))
        {
            Position = new Vector2(100, 100),
            Velocity = new Vector2(25, -10),
            IsAlive = true
        };

        bullet.Update(new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.That(bullet.Position.X, Is.EqualTo(125));
        Assert.That(bullet.Position.Y, Is.EqualTo(90));
        Assert.That(bullet.IsAlive, Is.True);
    }

    [Test]
    public void BulletMovementFactory_LinearStrategy_DeactivatesOutOfBoundsBullets()
    {
        var bullet = new Bullet(null!, BulletMovementFactory.Create(BulletMovementType.Linear))
        {
            Position = new Vector2(GameConfig.Playfield.Right + 150, 100),
            Velocity = Vector2.Zero,
            IsAlive = true
        };

        bullet.Update(new GameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.That(bullet.IsAlive, Is.False);
    }

    [Test]
    public void BulletMovementFactory_SinusoidalStrategy_AddsLateralDeviation()
    {
        var bullet = new Bullet(null!, BulletMovementFactory.Create(BulletMovementType.Sinusoidal))
        {
            Position = new Vector2(300, 300),
            Velocity = new Vector2(0, -200),
            IsAlive = true
        };

        bullet.Update(new GameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)));

        Assert.That(Math.Abs(bullet.Position.X - 300f), Is.GreaterThan(1f));
        Assert.That(bullet.Position.Y, Is.LessThan(300f));
        Assert.That(bullet.IsAlive, Is.True);
    }

    [Test]
    public void BulletMovementFactory_SinusoidalStrategy_DeactivatesOutOfBoundsBullets()
    {
        var bullet = new Bullet(null!, BulletMovementFactory.Create(BulletMovementType.Sinusoidal))
        {
            Position = new Vector2(GameConfig.Playfield.Right + 150, 100),
            Velocity = new Vector2(0, -150),
            IsAlive = true
        };

        bullet.Update(new GameTime(TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.2)));

        Assert.That(bullet.IsAlive, Is.False);
    }
}
