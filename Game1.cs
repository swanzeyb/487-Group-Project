using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;
using Entities;
using System;
using System.Collections.Generic;

namespace _487_Group_Project;

public enum  GamePhase
{
    Phase1,
    Phase2,
    Phase3,
    Phase4
}

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

    // Spawn logic and phase management
    private double _spawnTimer = 0;
    private Random rand = new Random();
    private double _phaseTimeElapsed = 0;
    private GamePhase _currentPhase = GamePhase.Phase1;
    private bool _bossSpawnedThisPhase = false;

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
    }

    protected override void Update(GameTime gameTime)
    {
        _input.Update();
        if (_input.Down(Keys.Escape))
            Exit();

        _player.Update(gameTime);

        UpdatePhases(gameTime);

        _spawnTimer += gameTime.ElapsedGameTime.TotalSeconds;

        double spawnInterval = _currentPhase == GamePhase.Phase1 ? 1.5 : 1.0; // Faster spawns in later phases.

        if (_spawnTimer >= spawnInterval) // Spawns enemy in 1.5s intervals.
        {
            _spawnTimer = 0;
            SpawnEnemyForCurrentPhase();
        }

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

    private void  UpdatePhases(GameTime gameTime)
    {
        _phaseTimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;

        if (_currentPhase == GamePhase.Phase1 && _phaseTimeElapsed >= 5)
        {
            _currentPhase = GamePhase.Phase2;
            _phaseTimeElapsed = 0;
        }
        else if (_currentPhase == GamePhase.Phase2 && _phaseTimeElapsed >= 12)
        {
            _currentPhase = GamePhase.Phase3;
            _phaseTimeElapsed = 0;
            _bossSpawnedThisPhase = false; // Reset boss spawn flag for new phase.
        }
        else if (_currentPhase == GamePhase.Phase3 && _phaseTimeElapsed >= 20)
        {
            _currentPhase = GamePhase.Phase4;
            _phaseTimeElapsed = 0;
            _bossSpawnedThisPhase = false;
        }
    }

    private void SpawnEnemyForCurrentPhase()
    {
        float spawnX = rand.Next(GameConfig.Playfield.Left + 20, GameConfig.Playfield.Right - 20);
        Vector2 spawnPos = new Vector2(spawnX, GameConfig.Playfield.Top - 50);

        EnemyType typeSpawn = EnemyType.Grunt;
        Vector2 velocity = new Vector2(0, 100f);

        switch (_currentPhase)
        {
            case GamePhase.Phase1:
                typeSpawn = EnemyType.Grunt;
                break;
            case GamePhase.Phase2:
                typeSpawn = rand.NextDouble() > 0.7 ? EnemyType.BetterGrunt : EnemyType.Grunt;
                break;
            case GamePhase.Phase3:
                if (!_bossSpawnedThisPhase && _phaseTimeElapsed > 5)
                {
                    typeSpawn = EnemyType.MidBoss;
                    _bossSpawnedThisPhase = true;
                    velocity.Y = 50f; // Boss moves slower.
                    spawnPos.X = GameConfig.Playfield.Center.X; // Boss spawns in the center.
                }
                else
                {
                    typeSpawn = EnemyType.Grunt;
                }
                break;
            case GamePhase.Phase4:
                if (!_bossSpawnedThisPhase && _phaseTimeElapsed > 5) 
                {
                    typeSpawn = EnemyType.FinalBoss;
                    _bossSpawnedThisPhase = true;
                    velocity.Y = 25f;
                    spawnPos.X = GameConfig.Playfield.Center.X;
                }
                else
                {
                    typeSpawn = EnemyType.Grunt;
                    velocity.Y = 130f; // Faster grunts in final phase.
                }
                break;

        }

        _enemies.Add(new Enemy(_drawer, typeSpawn, spawnPos, velocity));
    }
}