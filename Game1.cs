using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;
using Entities;
using Level;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _487_Group_Project;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics = null!;
    private SpriteBatch _spriteBatch = null!;

    // Core stuff
    private SimpleDrawer _drawer = null!;
    private InputState _input = null!;

    // Entities
    private Player _player = null!;
    private List<Enemy> _enemies = new List<Enemy>();

    // Level system (replaces old hardcoded phase logic)
    private LevelManager _levelManager = null!;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = GameConfig.ScreenWidth;
        _graphics.PreferredBackBufferHeight = GameConfig.ScreenHeight;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _drawer = new SimpleDrawer(GraphicsDevice);
        _input = new InputState();
        _player = new Player(_drawer, _input);

        // Load the level from JSON
        _levelManager = new LevelManager();
        _levelManager.OnSpawnEnemy += HandleSpawnEnemy;
        _levelManager.OnPhaseChanged += HandlePhaseChanged;

        string levelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Levels", "stage1.json");
        _levelManager.LoadLevel(levelPath);
    }

    protected override void Update(GameTime gameTime)
    {
        _input.Update();
        if (_input.Down(Keys.Escape))
            Exit();

        _player.Update(gameTime);

        // Drive wave/phase progression — pass living enemy count for "allKilled" conditions
        int livingCount = _enemies.Count(e => e.IsAlive);
        _levelManager.Update(gameTime, livingCount);

        // Iterate backwards to remove dead enemies from the list.
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            _enemies[i].Update(gameTime);

            if (!_enemies[i].IsAlive)
            {
                _enemies.RemoveAt(i);
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw playfield boundary
        _drawer.DrawRectOutline(_spriteBatch, GameConfig.Playfield, thickness: 3, color: Color.DarkGray);

        // Draw player
        _player.Draw(_spriteBatch);

        // Draw enemies.
        foreach (var enemy in _enemies)
        {
            enemy.Draw(_spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    // ── Level event handlers ────────────────────────────────

    private void HandleSpawnEnemy(object sender, SpawnEnemyEventArgs e)
    {
        _enemies.Add(EnemyFactory.Create(_drawer, e.EnemyType, e.Position, e.Velocity));
    }

    private void HandlePhaseChanged(object sender, int newPhaseIndex)
    {
        // Hook for future UI updates, screen-clear effects, etc.
        System.Diagnostics.Debug.WriteLine($"Phase changed to index {newPhaseIndex}: {_levelManager.CurrentPhaseName}");
    }
}