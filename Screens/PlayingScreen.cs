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

    private readonly Texture2D _playerSprite;
    private readonly Texture2D _gruntSprite;
    private readonly Texture2D _betterGruntSprite;
    private readonly Texture2D _midBossSprite;
    private readonly Texture2D _finalBossSprite;
    private readonly Texture2D _panelBorderSprite;

    private Player _player;
    private List<Enemy> _enemies = new();
    private BulletManager _bulletManager;
    private LevelManager _levelManager;
    private HudPanelData _hudData = new();
    private float _timeSinceLastShot = 0f;
    private const float PlayerFireRate = 0.1f; // 10 shots per second

    public Action OnPause;
    public Action OnGameOver;
    public Action OnVictory;

    public int CurrentScore => _scoreManager.Score;

    public PlayingScreen(SimpleDrawer drawer, InputState input, KeyBindings keyBindings,
                         ScoreManager scoreManager, SpriteFont defaultFont, 
                         Texture2D playerSprite, Texture2D gruntSprite, Texture2D betterGruntSprite,
                         Texture2D midBossSprite, Texture2D finalBossSprite,
                         Texture2D panelBorderSprite)
    {
        _drawer = drawer;
        _input = input;
        _keyBindings = keyBindings;
        _scoreManager = scoreManager;
        _hudPanel = new HudPanel(defaultFont, drawer);
        _playerSprite = playerSprite;
        _gruntSprite = gruntSprite;
        _betterGruntSprite = betterGruntSprite;
        _midBossSprite = midBossSprite;
        _finalBossSprite = finalBossSprite;
        _panelBorderSprite = panelBorderSprite;
    }

    public void OnEnter()
    {
        _player = new Player(_drawer, _input, _playerSprite);
        _enemies.Clear();
        _bulletManager = new BulletManager(_drawer);
        _scoreManager.Reset();
        _hudData = new HudPanelData();

        if (GameConfig.IsDebugMode)
        {
            // Debug mode: spawn single enemy at center
            var enemyPos = new Vector2(GameConfig.Playfield.Center.X, GameConfig.Playfield.Top + 50);
            var enemyVel = new Vector2(0, 50); // Slow downward movement
            
            Texture2D debugSprite = GameConfig.SelectedEnemyType switch
            {
                EnemyType.Grunt => _gruntSprite,
                EnemyType.BetterGrunt => _betterGruntSprite,
                EnemyType.MidBoss => _midBossSprite,
                EnemyType.FinalBoss => _finalBossSprite,
                _ => _gruntSprite
            };
            
            _enemies.Add(EnemyFactory.Create(_drawer, GameConfig.SelectedEnemyType, enemyPos, enemyVel, _bulletManager, "linear", debugSprite));
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

        // Handle player shooting (continuous fire)
        _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (input.Down(_keyBindings.GetKey("Shoot")) && _timeSinceLastShot >= PlayerFireRate)
        {
            var bulletVelocity = new Vector2(0, -500); // Shoot upward
            _bulletManager.FireBullet(_player.Position, bulletVelocity, damage: 1, isPlayerFired: true);
            _timeSinceLastShot = 0f;
        }

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

        // Check collisions between player bullets and enemies
        foreach (var bullet in _bulletManager.ActiveBullets.ToList())
        {
            if (bullet.IsPlayerFired)
            {
                foreach (var enemy in _enemies)
                {
                    if (enemy.IsAlive && enemy.Bounds.Intersects(bullet.Bounds))
                    {
                        enemy.TakeDamage(bullet.Damage);
                        bullet.IsAlive = false;
                        
                        // Award points when enemy dies
                        if (!enemy.IsAlive)
                        {
                            _scoreManager.AddScore(100); 
                        }
                        
                        break; // Bullet hit, move to next bullet
                    }
                }
            }
        }

        // Check collisions between bullets and player
        foreach (var bullet in _bulletManager.ActiveBullets.ToList())
        {
            if (!bullet.IsPlayerFired && _player.Bounds.Intersects(bullet.Bounds))
            {
                // Player hit by enemy bullet
                _player.TakeDamage(bullet.Damage);
                bullet.IsAlive = false;
                
                if (!_player.IsAlive)
                {
                    OnGameOver?.Invoke();
                    return;
                }
            }
        }

        // Check collisions between lasers and player (using distance-to-segment test)
        foreach (var laser in _bulletManager.ActiveLasers)
        {
            if (laser.IsActive && LaserIntersectsPlayer(laser, _player.Bounds))
            {
                OnGameOver?.Invoke();
                return;
            }
        }

        // Update HUD data
        _hudData.Score = _scoreManager.Score;
        _hudData.PlayerHP = _player.HP;
        
        // Update boss health bar if a boss is present
        var boss = _enemies.FirstOrDefault(e => e.Type == EnemyType.MidBoss || e.Type == EnemyType.FinalBoss);
        if (boss != null)
        {
            // Calculate max HP based on boss type
            int maxHP = boss.Type == EnemyType.MidBoss ? 100 : 300;
            _hudData.BossHealthPercent = boss.HP / (float)maxHP;
        }
        else
        {
            _hudData.BossHealthPercent = 1f;
        }
        
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
        _player.Draw(spriteBatch);

        // Draw enemies
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch);
        }

        // Draw bullets
        _bulletManager.Draw(spriteBatch);

        // Draw playfield border from panel-border-015 using 9-slice to match original visuals
        if (_panelBorderSprite != null)
        {
            DrawPlayfieldBorder(spriteBatch);
        }
        else
        {
            drawer.DrawRectOutline(spriteBatch, GameConfig.Playfield, thickness: 3, color: Color.DarkGray);
        }

        _hudPanel.Draw(spriteBatch, _hudData);
    }

    private void DrawPlayfieldBorder(SpriteBatch spriteBatch)
    {
        const int borderPx = 2;
        var src = _panelBorderSprite.Bounds;
        var dst = GameConfig.Playfield;

        // corners
        spriteBatch.Draw(_panelBorderSprite, new Rectangle(dst.Left, dst.Top, borderPx, borderPx), new Rectangle(0, 0, borderPx, borderPx), Color.White);
        spriteBatch.Draw(_panelBorderSprite, new Rectangle(dst.Right - borderPx, dst.Top, borderPx, borderPx), new Rectangle(src.Width - borderPx, 0, borderPx, borderPx), Color.White);
        spriteBatch.Draw(_panelBorderSprite, new Rectangle(dst.Left, dst.Bottom - borderPx, borderPx, borderPx), new Rectangle(0, src.Height - borderPx, borderPx, borderPx), Color.White);
        spriteBatch.Draw(_panelBorderSprite, new Rectangle(dst.Right - borderPx, dst.Bottom - borderPx, borderPx, borderPx), new Rectangle(src.Width - borderPx, src.Height - borderPx, borderPx, borderPx), Color.White);

        // edges
        spriteBatch.Draw(_panelBorderSprite, new Rectangle(dst.Left + borderPx, dst.Top, dst.Width - borderPx * 2, borderPx), new Rectangle(borderPx, 0, src.Width - borderPx * 2, borderPx), Color.White);
        spriteBatch.Draw(_panelBorderSprite, new Rectangle(dst.Left + borderPx, dst.Bottom - borderPx, dst.Width - borderPx * 2, borderPx), new Rectangle(borderPx, src.Height - borderPx, src.Width - borderPx * 2, borderPx), Color.White);
        spriteBatch.Draw(_panelBorderSprite, new Rectangle(dst.Left, dst.Top + borderPx, borderPx, dst.Height - borderPx * 2), new Rectangle(0, borderPx, borderPx, src.Height - borderPx * 2), Color.White);
        spriteBatch.Draw(_panelBorderSprite, new Rectangle(dst.Right - borderPx, dst.Top + borderPx, borderPx, dst.Height - borderPx * 2), new Rectangle(src.Width - borderPx, borderPx, borderPx, src.Height - borderPx * 2), Color.White);
    }

    private void HandleSpawnEnemy(object sender, SpawnEnemyEventArgs e)
    {
        Texture2D spriteForType = e.EnemyType switch
        {
            EnemyType.Grunt => _gruntSprite,
            EnemyType.BetterGrunt => _betterGruntSprite,
            EnemyType.MidBoss => _midBossSprite,
            EnemyType.FinalBoss => _finalBossSprite,
            _ => _gruntSprite
        };
        _enemies.Add(EnemyFactory.Create(_drawer, e.EnemyType, e.Position, e.Velocity, _bulletManager, e.MovementPattern, spriteForType));
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
