using Core;
using Entities;
using Entities.Movement;
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

    [Test]
    public void Enemy_SinusoidalPattern_BouncesAtLeftBoundary()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(45, 120),
            new Vector2(0, 90),
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "sinusoidal");

        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)), Vector2.Zero);

        Assert.That(enemy.Position.X, Is.GreaterThanOrEqualTo(GameConfig.Playfield.Left + 20f));
        Assert.That(enemy.IsAlive, Is.True);
    }

    [Test]
    public void Enemy_SinusoidalPattern_BouncesAtRightBoundary()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(615, 120),
            new Vector2(0, 90),
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "sinusoidal");

        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5)), Vector2.Zero);

        Assert.That(enemy.Position.X, Is.LessThanOrEqualTo(GameConfig.Playfield.Right - 20f));
        Assert.That(enemy.IsAlive, Is.True);
    }

    [Test]
    public void Enemy_SinusoidalPattern_StillDespawnsThroughBottom()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(320, 505),
            new Vector2(0, 220),
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "sinusoidal");

        enemy.Update(new GameTime(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0)), Vector2.Zero);

        Assert.That(enemy.IsAlive, Is.False);
    }

    [Test]
    public void Enemy_LinearPattern_BouncesAtRightBoundary_WithNonzeroVelocityX()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(GameConfig.Playfield.Right - 22, 120),
            new Vector2(200, 0),
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "linear");

        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.2)), Vector2.Zero);
        float firstX = enemy.Position.X;
        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.2)), Vector2.Zero);
        float secondX = enemy.Position.X;

        Assert.That(firstX, Is.EqualTo(GameConfig.Playfield.Right - 20f).Within(0.001f));
        Assert.That(secondX, Is.LessThan(firstX));
    }

    [Test]
    public void Enemy_SinusoidalPattern_WithNonzeroVelocityX_DoesNotStickToRightBoundary()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.MidBoss,
            new Vector2(570, 200),
            new Vector2(100, 0),
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "sinusoidal");

        float rightLimit = GameConfig.Playfield.Right - 50f;
        bool touchedRight = false;
        bool movedAwayAfterTouch = false;

        for (int i = 0; i < 24; i++)
        {
            enemy.Update(new GameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)), Vector2.Zero);
            Assert.That(enemy.Position.X, Is.LessThanOrEqualTo(rightLimit + 0.01f));
            Assert.That(enemy.Position.X, Is.GreaterThanOrEqualTo(GameConfig.Playfield.Left + 50f - 0.01f));

            if (Math.Abs(enemy.Position.X - rightLimit) <= 0.01f)
            {
                touchedRight = true;
            }

            if (touchedRight && enemy.Position.X < rightLimit - 1f)
            {
                movedAwayAfterTouch = true;
                break;
            }
        }

        Assert.That(touchedRight, Is.True);
        Assert.That(movedAwayAfterTouch, Is.True);
    }

    [Test]
    public void Enemy_CircularPattern_PositionChangesEachUpdate()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(300, 200),
            Vector2.Zero,
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "circular");

        Vector2 first = enemy.Position;
        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)), Vector2.Zero);
        Vector2 second = enemy.Position;
        enemy.Update(new GameTime(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1)), Vector2.Zero);
        Vector2 third = enemy.Position;

        Assert.That(second, Is.Not.EqualTo(first));
        Assert.That(third, Is.Not.EqualTo(second));
    }

    [Test]
    public void Enemy_CircularPattern_StaysWithinPlayfieldBounds()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(300, 200),
            Vector2.Zero,
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "circular");

        for (int i = 0; i < 100; i++)
        {
            enemy.Update(new GameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05)), Vector2.Zero);
            Assert.That(enemy.Position.X, Is.GreaterThan(GameConfig.Playfield.Left), $"X out of bounds at step {i}");
            Assert.That(enemy.Position.X, Is.LessThan(GameConfig.Playfield.Right), $"X out of bounds at step {i}");
            Assert.That(enemy.Position.Y, Is.GreaterThan(GameConfig.Playfield.Top), $"Y out of bounds at step {i}");
            Assert.That(enemy.Position.Y, Is.LessThan(GameConfig.Playfield.Bottom), $"Y out of bounds at step {i}");
        }
    }

    [Test]
    public void Enemy_CircularPattern_NeverDespawns()
    {
        var enemy = new Enemy(
            null!,
            EnemyType.Grunt,
            new Vector2(300, 200),
            Vector2.Zero,
            new NoOpShootingStrategy(),
            null!,
            movementPattern: "circular");

        for (int i = 0; i < 200; i++)
            enemy.Update(new GameTime(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05)), Vector2.Zero);

        Assert.That(enemy.IsAlive, Is.True);
    }

    [Test]
    public void Enemy_CircularPattern_CaseInsensitive()
    {
        var strategy = EnemyMovementFactory.Create("Circular");
        Assert.That(strategy, Is.Not.Null);
    }

    private sealed class NoOpShootingStrategy : IShootingStrategy
    {
        public void Update(GameTime gameTime, Vector2 enemyPosition, Vector2 playerPosition, BulletManager bulletManager)
        {
        }
    }
}
