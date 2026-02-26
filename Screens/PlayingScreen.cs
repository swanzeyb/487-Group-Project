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
        _scoreManager.Reset();
        _hudData = new HudPanelData();

        _levelManager = new LevelManager();
        _levelManager.OnSpawnEnemy += HandleSpawnEnemy;
        _levelManager.OnPhaseChanged += HandlePhaseChanged;

        string levelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Levels", "stage1.json");
        _levelManager.LoadLevel(levelPath);
    }

    public void OnExit()
    {
        if (_levelManager != null)
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

        _player.Update(gameTime);

        int livingCount = _enemies.Count(e => e.IsAlive);
        _levelManager.Update(gameTime, livingCount);

        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            _enemies[i].Update(gameTime);
            if (!_enemies[i].IsAlive)
            {
                _enemies.RemoveAt(i);
            }
        }

        // Update HUD data
        _hudData.Score = _scoreManager.Score;
        _hudData.PhaseName = _levelManager.CurrentPhaseName;

        // Check victory
        if (_levelManager.IsLevelComplete && _enemies.Count(e => e.IsAlive) == 0)
        {
            OnVictory?.Invoke();
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

        // Draw HUD
        _hudPanel.Draw(spriteBatch, _hudData);
    }

    private void HandleSpawnEnemy(object sender, SpawnEnemyEventArgs e)
    {
        _enemies.Add(EnemyFactory.Create(_drawer, e.EnemyType, e.Position, e.Velocity));
    }

    private void HandlePhaseChanged(object sender, int newPhaseIndex)
    {
        _hudData.PhaseName = _levelManager.CurrentPhaseName;
    }
}
