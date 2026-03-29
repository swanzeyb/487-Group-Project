using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core;
using Entities;
using Level;
using UI;

namespace Screens;

public class PlayingScreen : IScreen
{
    private readonly SimpleDrawer _drawer;
    private readonly InputState _input;
    private readonly KeyBindings _keyBindings;
    private readonly ScoreManager _scoreManager;
    private readonly HudPanel _hudPanel;

    private Player _player;
    private List<Enemy> _enemies = new();
    private BulletManager _bulletManager;
    private LevelManager _levelManager;
    private HudPanelData _hudData = new();

    public Action OnPause;
    public Action OnGameOver;
    public Action OnVictory;

    public int CurrentScore => _scoreManager.Score;

    public PlayingScreen(SimpleDrawer drawer, InputState input, KeyBindings keyBindings,
                         ScoreManager scoreManager, SpriteFont defaultFont)
    {
        _drawer = drawer;
        _input = input;
        _keyBindings = keyBindings;
        _scoreManager = scoreManager;
        _hudPanel = new HudPanel(defaultFont, drawer);
    }

    public void OnEnter()
    {
        _player = new Player(_drawer, _input);
        _enemies.Clear();
        _bulletManager = new BulletManager(_drawer);
        _scoreManager.Reset();
        _hudData = new HudPanelData();

        if (GameConfig.IsDebugMode)
        {
            // Debug mode: spawn single enemy at center
            var enemyPos = new Vector2(GameConfig.Playfield.Center.X, GameConfig.Playfield.Top + 50);
            var enemyVel = new Vector2(0, 50); // Slow downward movement
            _enemies.Add(EnemyFactory.Create(_drawer, GameConfig.SelectedEnemyType, enemyPos, enemyVel, _bulletManager));
            _hudData.PhaseName = $"Debug: {GameConfig.SelectedEnemyType}";
        }
        else
        {
            // Normal mode: load level
            _levelManager = new LevelManager();
            _levelManager.OnSpawnEnemy += HandleSpawnEnemy;
            _levelManager.OnPhaseChanged += HandlePhaseChanged;

            string levelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Levels", "stage1.json");
            _levelManager.LoadLevel(levelPath);
        }
    }

    public void OnExit()
    {
        if (!GameConfig.IsDebugMode && _levelManager != null)
        {
            _levelManager.OnSpawnEnemy -= HandleSpawnEnemy;
            _levelManager.OnPhaseChanged -= HandlePhaseChanged;
        }
    }

    public void Update(GameTime gameTime, InputState input)
    {
        if (input.Pressed(_keyBindings.GetKey("Pause")))
        {
            OnPause?.Invoke();
            return;
        }

        _player.Update(gameTime, _player.Position);

        if (!GameConfig.IsDebugMode)
        {
            int livingCount = _enemies.Count(e => e.IsAlive);
            _levelManager.Update(gameTime, livingCount);
        }

        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            _enemies[i].Update(gameTime, _player.Position);
            if (!_enemies[i].IsAlive)
            {
                _enemies.RemoveAt(i);
            }
        }

        // Update bullets
        _bulletManager.Update(gameTime);

        // Check collisions between bullets and player
        foreach (var bullet in _bulletManager.ActiveBullets)
        {
            if (bullet.IsAlive && _player.Bounds.Intersects(bullet.Bounds))
            {
                // Apply damage to the player
                _player.TakeDamage(bullet.Damage);

                // Destroy the bullet so it doesn't hit multiple times
                bullet.IsAlive = false;
            }
        }

        // Check collisions between lasers and player (using distance-to-segment test)
        foreach (var laser in _bulletManager.ActiveLasers)
        {
            if (laser.IsActive && LaserIntersectsPlayer(laser, _player.Bounds))
            {
                _player.TakeDamage(_player.MaxHealth);
            }
        }

        // Check if player died during this frame
        if (!_player.IsAlive)
        {
            OnGameOver?.Invoke();
            return;
        }

        // Update HUD data
        _hudData.Score = _scoreManager.Score;

        // Update health data
        _hudData.PlayerHealth = _player.CurrentHealth;
        _hudData.PlayerMaxHealth = _player.MaxHealth;

        if (!GameConfig.IsDebugMode)
        {
            _hudData.PhaseName = _levelManager.CurrentPhaseName;
        }

        // Check victory
        if (GameConfig.IsDebugMode)
        {
            if (_enemies.Count(e => e.IsAlive) == 0)
            {
                OnVictory?.Invoke();
            }
        }
        else
        {
            if (_levelManager.IsLevelComplete && _enemies.Count(e => e.IsAlive) == 0)
            {
                OnVictory?.Invoke();
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, SimpleDrawer drawer)
    {
        // Draw playfield boundary
        drawer.DrawRectOutline(spriteBatch, GameConfig.Playfield, thickness: 3, color: Color.DarkGray);

        // Draw player
        _player.Draw(spriteBatch);

        // Draw enemies
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch);
        }

        // Draw bullets
        _bulletManager.Draw(spriteBatch);

        // Draw HUD
        _hudPanel.Draw(spriteBatch, _hudData);
    }

    private void HandleSpawnEnemy(object sender, SpawnEnemyEventArgs e)
    {
        _enemies.Add(EnemyFactory.Create(_drawer, e.EnemyType, e.Position, e.Velocity, _bulletManager));
    }

    // helper used during Update for more accurate laser collision tests
    private static bool LaserIntersectsPlayer(Laser laser, Rectangle playerBounds)
    {
        // treat player as a circle for simplicity
        Vector2 playerCenter = playerBounds.Center.ToVector2();
        float playerRadius = Math.Min(playerBounds.Width, playerBounds.Height) / 2f;
        Vector2 start = laser.StartPosition;
        Vector2 end = laser.EndPosition;
        Vector2 seg = end - start;
        float lenSq = seg.LengthSquared();
        if (lenSq < 0.0001f)
        {
            // degenerate segment, just compare to start point
            return Vector2.Distance(playerCenter, start) <= Laser.BeamThickness / 2 + playerRadius;
        }
        // project player centre onto segment
        float t = Vector2.Dot(playerCenter - start, seg) / lenSq;
        t = MathHelper.Clamp(t, 0f, 1f);
        Vector2 closest = start + seg * t;
        float distance = Vector2.Distance(playerCenter, closest);
        return distance <= Laser.BeamThickness / 2 + playerRadius;
    }

    private void HandlePhaseChanged(object sender, int newPhaseIndex)
    {
        _hudData.PhaseName = _levelManager.CurrentPhaseName;
    }
}
