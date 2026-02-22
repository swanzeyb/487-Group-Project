using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;
using Entities;
using System;
using System.Collections.Generic;

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

    // Spawn logic
    private double _spawnTimer = 0;
    private Random rand = new Random();

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
        if(_input.Down(Keys.Escape))
            Exit();
        
        _player.Update(gameTime);

        _spawnTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_spawnTimer >= 1.5) // Spawns enemy in 1.5s intervals.
        {
            _spawnTimer = 0;
            SpawnRandomEnemy();
        }

        // Iterate backwards to remove dead enemies from the list.
        for (int i = _enemies.Count -1; i >= 0; i--)
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

    private void SpawnRandomEnemy()
    {
        float spawnX = rand.Next(GameConfig.Playfield.Left + 20, GameConfig.Playfield.Right - 20);
        Vector2 spawnPos = new Vector2(spawnX, GameConfig.Playfield.Top - 50);

        EnemyType randomType = (EnemyType)rand.Next(0, 4);

        Vector2 velocity = new Vector2(0, 100f);

        if (randomType == EnemyType.MidBoss || randomType == EnemyType.FinalBoss)
        {
            velocity.Y = 50f;
        }

        _enemies.Add(new Enemy(_drawer, randomType, spawnPos, velocity));
    }
}
