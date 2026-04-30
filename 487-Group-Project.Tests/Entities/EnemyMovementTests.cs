using Core;
using Entities;
using Microsoft.Xna.Framework;

namespace _487_Group_Project.Tests.Entities;

[TestFixture]
public class EnemyMovementTests
{
    [Test]
    public void Enemy_LinearPattern_WithZeroVelocityX_DoesNotShiftHorizontally()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(220, 100),
            new Vector2(0, 120),
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "linear");

        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)), Vector2.Zero);

        Assert.That(enemy.Position.X, Is.EqualTo(220f).Within(0.001f));
        Assert.That(enemy.Position.Y, Is.GreaterThan(100f));
    }

    [Test]
    public void Enemy_SinusoidalPattern_WithZeroVelocityX_ShiftsHorizontally()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(220, 100),
            new Vector2(0, 120),
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "sinusoidal");

        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)), Vector2.Zero);

        Assert.That(Math.Abs(enemy.Position.X - 220f), Is.GreaterThan(1f));
        Assert.That(enemy.Position.Y, Is.GreaterThan(100f));
    }

    [Test]
    public void Enemy_UnknownPattern_FallsBackToLinearBehavior()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(220, 100),
            new Vector2(0, 120),
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "unknown-pattern");

        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)), Vector2.Zero);

        Assert.That(enemy.Position.X, Is.EqualTo(220f).Within(0.001f));
    }

    [Test]
    public void Enemy_SinusoidalPattern_IsCaseInsensitive()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(220, 100),
            new Vector2(0, 120),
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "Sinusoidal");

        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)), Vector2.Zero);

        Assert.That(Math.Abs(enemy.Position.X - 220f), Is.GreaterThan(1f));
    }

    [Test]
    public void Enemy_SinusoidalPattern_WithHorizontalVelocity_ShiftsVertically()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.MidBoss,
            new Vector2(220, 120),
            new Vector2(80, 0),
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "sinusoidal");

        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)), Vector2.Zero);

        Assert.That(Math.Abs(enemy.Position.Y - 120f), Is.GreaterThan(1f));
        Assert.That(enemy.Position.X, Is.GreaterThan(220f));
    }

    private sealed class NoOpShootingStrategy : IShootingStrategy
    {
        public void Update(GameTime gameTime, Vector2 enemyPosition, Vector2 playerPosition, BulletManager bulletManager)
        {
        }
    }
}
