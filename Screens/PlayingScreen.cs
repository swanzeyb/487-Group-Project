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
    private sealed class BlueStar
    {
        public Vector2 Position;
        public int Size;
        public Color Tint;
    }

    private sealed class MeteorProjectile
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public float RotationSpeed;
        public float Scale;

        public Rectangle Bounds(Texture2D sprite)
        {
            int w = (int)(sprite.Width * Scale);
            int h = (int)(sprite.Height * Scale);
            return new Rectangle((int)Position.X - w / 2, (int)Position.Y - h / 2, w, h);
        }
    }

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
    private readonly Texture2D _playerBulletSprite;
    private readonly Texture2D _gruntBulletSprite;
    private readonly Texture2D _betterGruntBulletSprite;
    private readonly Texture2D _midBossBulletSprite;
    private readonly Texture2D _nebulaSprite;
    private readonly Texture2D _meteorSprite;
    private readonly Random _rng = new();
    private readonly List<BlueStar> _blueStars = new();
    private readonly List<MeteorProjectile> _meteors = new();

    private Player _player;
    private List<Enemy> _enemies = new();
    private BulletManager _bulletManager;
    private LevelManager _levelManager;
    private HudPanelData _hudData = new();
    private float _timeSinceLastShot = 0f;
    private const float PlayerFireRate = 0.1f; // 10 shots per second
    private float _bombVisualTimer = 0f;
    private float _bombInvulnerabilityTimer = 0f;
    private const float BombVisualDuration = 0.35f;
    private const float BombInvulnerabilityDuration = 0.75f;
    private const int StartingBombCount = 3;
    private const int BombDamage = 20;
    private float _meteorSpawnTimer = 0f;
    private const float MeteorSpawnInterval = 0.9f;
    private const int MeteorDamage = 50;

    public Action OnPause;
    public Action OnGameOver;
    public Action OnVictory;

    public int CurrentScore => _scoreManager.Score;

    public PlayingScreen(SimpleDrawer drawer, InputState input, KeyBindings keyBindings,
                         ScoreManager scoreManager, SpriteFont defaultFont, 
                         Texture2D playerSprite, Texture2D gruntSprite, Texture2D betterGruntSprite,
                         Texture2D midBossSprite, Texture2D finalBossSprite,
                         Texture2D panelBorderSprite,
                         Texture2D playerBulletSprite,
                         Texture2D gruntBulletSprite,
                         Texture2D betterGruntBulletSprite,
                         Texture2D midBossBulletSprite,
                         Texture2D nebulaSprite,
                         Texture2D meteorSprite)
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
        _playerBulletSprite = playerBulletSprite;
        _gruntBulletSprite = gruntBulletSprite;
        _betterGruntBulletSprite = betterGruntBulletSprite;
        _midBossBulletSprite = midBossBulletSprite;
        _nebulaSprite = nebulaSprite;
        _meteorSprite = meteorSprite;
    }

    public void OnEnter()
    {
        _player = new Player(_drawer, _input, _playerSprite);
        _enemies.Clear();
        _bulletManager = new BulletManager(
            _drawer,
            _playerBulletSprite,
            _gruntBulletSprite,
            _betterGruntBulletSprite,
            _midBossBulletSprite);
        _scoreManager.Reset();
        _hudData = new HudPanelData();
        _hudData.BombCount = StartingBombCount;
        _bombVisualTimer = 0f;
        _bombInvulnerabilityTimer = 0f;
        _meteorSpawnTimer = 0f;
        _meteors.Clear();
        InitializeBlueStars();

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
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (input.Pressed(_keyBindings.GetKey("Pause")))
        {
            OnPause?.Invoke();
            return;
        }

        if (_bombVisualTimer > 0f)
        {
            _bombVisualTimer = System.Math.Max(0f, _bombVisualTimer - dt);
        }

        if (_bombInvulnerabilityTimer > 0f)
        {
            _bombInvulnerabilityTimer = System.Math.Max(0f, _bombInvulnerabilityTimer - dt);
        }

        _player.Update(gameTime, _player.Position);

        // Handle player shooting (continuous fire)
        _timeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (input.Down(_keyBindings.GetKey("Shoot")) && _timeSinceLastShot >= PlayerFireRate)
        {
            var bulletVelocity = new Vector2(0, -500); // Shoot upward
            _bulletManager.FireBullet(
                _player.Position,
                bulletVelocity,
                damage: 1,
                isPlayerFired: true,
                visualType: BulletVisualType.Player);
            _timeSinceLastShot = 0f;
        }

        if (input.Pressed(_keyBindings.GetKey("Bomb")) && _hudData.BombCount > 0)
        {
            ActivateBomb();
        }

        UpdateMeteors(dt);

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
            if (_bombInvulnerabilityTimer <= 0f && !bullet.IsPlayerFired && _player.Bounds.Intersects(bullet.Bounds))
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
            if (_bombInvulnerabilityTimer <= 0f && laser.IsActive && LaserIntersectsPlayer(laser, _player.Bounds))
            {
                OnGameOver?.Invoke();
                return;
            }
        }

        for (int i = _meteors.Count - 1; i >= 0; i--)
        {
            if (_bombInvulnerabilityTimer <= 0f && _player.Bounds.Intersects(_meteors[i].Bounds(_meteorSprite)))
            {
                _player.TakeDamage(MeteorDamage);
                _meteors.RemoveAt(i);

                if (!_player.IsAlive)
                {
                    OnGameOver?.Invoke();
                    return;
                }
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
        DrawSpaceBackground(spriteBatch);

        _player.Draw(spriteBatch);

        // Draw enemies
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch);
        }

        // Draw bullets
        _bulletManager.Draw(spriteBatch);

        foreach (var meteor in _meteors)
        {
            var dest = meteor.Bounds(_meteorSprite);
            var origin = new Vector2(_meteorSprite.Width / 2f, _meteorSprite.Height / 2f);
            spriteBatch.Draw(_meteorSprite, meteor.Position, null, Color.White, meteor.Rotation, origin, meteor.Scale, SpriteEffects.None, 0f);
        }

        if (_bombVisualTimer > 0f)
        {
            float progress = 1f - (_bombVisualTimer / BombVisualDuration);
            float radius = 30f + progress * 260f;
            int alpha = (int)(140f * (1f - progress));
            alpha = System.Math.Clamp(alpha, 0, 180);
            Color ringColor = new Color(255, 235, 150, alpha);
            drawer.DrawCircle(spriteBatch, _player.Position, radius, ringColor, segments: 48, thickness: 4);
        }

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

    private void ActivateBomb()
    {
        _hudData.BombCount -= 1;
        _bombVisualTimer = BombVisualDuration;
        _bombInvulnerabilityTimer = BombInvulnerabilityDuration;

        _bulletManager.ClearEnemyProjectiles();
        _meteors.Clear();

        foreach (var enemy in _enemies)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }

            bool wasAlive = enemy.IsAlive;
            enemy.TakeDamage(BombDamage);
            if (wasAlive && !enemy.IsAlive)
            {
                _scoreManager.AddScore(100);
            }
        }
    }

    private void DrawSpaceBackground(SpriteBatch spriteBatch)
    {
        _drawer.DrawRect(spriteBatch, GameConfig.Playfield, new Color(8, 10, 24));

        foreach (var star in _blueStars)
        {
            _drawer.DrawRect(spriteBatch, new Rectangle((int)star.Position.X, (int)star.Position.Y, star.Size, star.Size), star.Tint);
        }
    }

    private void InitializeBlueStars()
    {
        _blueStars.Clear();

        const int starCount = 220;
        for (int i = 0; i < starCount; i++)
        {
            int size = _rng.NextDouble() < 0.82 ? 1 : 2;
            int alpha = _rng.Next(120, 220);
            int blueBoost = _rng.Next(0, 30);
            int greenBoost = _rng.Next(0, 18);

            _blueStars.Add(new BlueStar
            {
                Position = new Vector2(
                    _rng.Next(GameConfig.Playfield.Left, GameConfig.Playfield.Right),
                    _rng.Next(GameConfig.Playfield.Top, GameConfig.Playfield.Bottom)),
                Size = size,
                Tint = new Color(110 + greenBoost, 170 + greenBoost, 230 + blueBoost, alpha)
            });
        }
    }

    private void UpdateMeteors(float dt)
    {
        bool meteorEnabled = IsMeteorPhase();
        if (meteorEnabled)
        {
            _meteorSpawnTimer += dt;
            if (_meteorSpawnTimer >= MeteorSpawnInterval)
            {
                _meteorSpawnTimer = 0f;
                SpawnMeteor();
            }
        }
        else
        {
            _meteorSpawnTimer = 0f;
            if (_meteors.Count > 0)
            {
                _meteors.Clear();
            }
            return;
        }

        for (int i = _meteors.Count - 1; i >= 0; i--)
        {
            var meteor = _meteors[i];
            meteor.Position += meteor.Velocity * dt;
            meteor.Rotation += meteor.RotationSpeed * dt;
            if (meteor.Position.Y > GameConfig.Playfield.Bottom + 80)
            {
                _meteors.RemoveAt(i);
            }
        }
    }

    private void SpawnMeteor()
    {
        float x = _rng.Next(GameConfig.Playfield.Left + 20, GameConfig.Playfield.Right - 20);
        float y = GameConfig.Playfield.Top - 50;
        float speedY = 210f + (float)_rng.NextDouble() * 120f;
        float speedX = -45f + (float)_rng.NextDouble() * 90f;

        _meteors.Add(new MeteorProjectile
        {
            Position = new Vector2(x, y),
            Velocity = new Vector2(speedX, speedY),
            Rotation = 0f,
            RotationSpeed = -2.2f + (float)_rng.NextDouble() * 4.4f,
            Scale = 0.55f + (float)_rng.NextDouble() * 0.5f
        });
    }

    private bool IsMeteorPhase()
    {
        if (GameConfig.IsDebugMode)
        {
            return GameConfig.SelectedEnemyType == EnemyType.MidBoss || GameConfig.SelectedEnemyType == EnemyType.FinalBoss;
        }

        string phase = _hudData.PhaseName?.ToLowerInvariant() ?? string.Empty;
        return phase.Contains("mid boss") || phase.Contains("final boss");
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
