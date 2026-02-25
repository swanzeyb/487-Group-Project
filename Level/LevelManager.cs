using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Entities;
using Microsoft.Xna.Framework;

namespace Level;

/// <summary>
/// Event args passed when the LevelManager requests an enemy spawn.
/// Game1 subscribes and creates the actual Entity via EnemyFactory.
/// </summary>
public class SpawnEnemyEventArgs : EventArgs
{
    public EnemyType EnemyType { get; }
    public Vector2 Position { get; }
    public Vector2 Velocity { get; }
    public string MovementPattern { get; }

    public SpawnEnemyEventArgs(EnemyType type, Vector2 position, Vector2 velocity, string movementPattern)
    {
        EnemyType = type;
        Position = position;
        Velocity = velocity;
        MovementPattern = movementPattern;
    }
}

/// <summary>
/// Reads a JSON level file and drives phase/wave progression at runtime.
/// Completely decoupled from entity creation — communicates via events.
/// </summary>
public class LevelManager
{
    // ── Events ──────────────────────────────────────────────
    /// <summary>Fired each time an enemy should be spawned.</summary>
    public event EventHandler<SpawnEnemyEventArgs> OnSpawnEnemy;

    /// <summary>Fired when phases advance (index of new phase).</summary>
    public event EventHandler<int> OnPhaseChanged;

    // ── Public state ────────────────────────────────────────
    public bool IsLevelComplete { get; private set; }
    public string CurrentPhaseName => _level != null && _phaseIndex < _level.Phases.Count
        ? _level.Phases[_phaseIndex].Name
        : "";

    public int CurrentPhaseIndex => _phaseIndex;

    // ── Private state ───────────────────────────────────────
    private LevelDefinition _level;
    private int _phaseIndex;
    private int _waveIndex;

    // Per-wave tracking
    private int _waveSpawnedCount;      // how many enemies have been spawned so far in this wave
    private int _waveKilledCount;       // tracked externally via ReportKill()
    private double _waveElapsed;        // seconds since wave started
    private double _spawnTimer;         // seconds since last spawn in this wave
    private bool _waveStartDelayDone;   // have we waited the startDelay?
    private bool _allSpawned;           // have all enemies for this wave been spawned?
    private double _postSpawnCooldown;  // timer after last spawn (for "timer" advance)

    private readonly Random _rand = new();

    // ── Load ────────────────────────────────────────────────

    /// <summary>Load a level from a JSON file path.</summary>
    public void LoadLevel(string jsonPath)
    {
        string json = File.ReadAllText(jsonPath);
        _level = JsonSerializer.Deserialize<LevelDefinition>(json)
                 ?? throw new InvalidOperationException($"Failed to deserialize level: {jsonPath}");

        _phaseIndex = 0;
        _waveIndex = 0;
        IsLevelComplete = false;
        ResetWaveState();
    }

    // ── Tick ────────────────────────────────────────────────

    /// <summary>
    /// Call every frame from Game1.Update().
    /// <paramref name="livingEnemyCount"/> = number of enemies currently alive
    /// that were spawned by this wave (or all living enemies if you keep it simple).
    /// </summary>
    public void Update(GameTime gameTime, int livingEnemyCount)
    {
        if (IsLevelComplete || _level == null)
            return;

        if (_phaseIndex >= _level.Phases.Count)
        {
            IsLevelComplete = true;
            return;
        }

        var phase = _level.Phases[_phaseIndex];

        if (_waveIndex >= phase.Waves.Count)
        {
            // Phase exhausted → advance
            _phaseIndex++;
            _waveIndex = 0;
            ResetWaveState();

            if (_phaseIndex >= _level.Phases.Count)
            {
                IsLevelComplete = true;
                return;
            }

            OnPhaseChanged?.Invoke(this, _phaseIndex);
            return;
        }

        var wave = phase.Waves[_waveIndex];
        double dt = gameTime.ElapsedGameTime.TotalSeconds;
        _waveElapsed += dt;

        // ── Start delay ──
        if (!_waveStartDelayDone)
        {
            if (_waveElapsed * 1000 < wave.StartDelayMs)
                return;
            _waveStartDelayDone = true;
            _spawnTimer = wave.SpawnIntervalMs / 1000.0; // spawn immediately on first tick after delay
        }

        // ── Spawn enemies ──
        if (_waveSpawnedCount < wave.Count)
        {
            _spawnTimer += dt;
            double interval = wave.SpawnIntervalMs / 1000.0;

            while (_spawnTimer >= interval && _waveSpawnedCount < wave.Count)
            {
                _spawnTimer -= interval;
                SpawnFromWave(wave);
            }
        }
        else if (!_allSpawned)
        {
            _allSpawned = true;
            _postSpawnCooldown = 0;
        }

        // ── Advance condition ──
        if (_allSpawned)
        {
            bool canAdvance = false;

            switch (wave.AdvanceCondition.ToLowerInvariant())
            {
                case "allkilled":
                    canAdvance = livingEnemyCount <= 0;
                    break;

                case "timer":
                default:
                    _postSpawnCooldown += dt;
                    canAdvance = _postSpawnCooldown >= wave.AdvanceCooldownSec;
                    break;
            }

            if (canAdvance)
            {
                _waveIndex++;
                ResetWaveState();
            }
        }
    }

    /// <summary>
    /// Optional: call when an enemy is killed so wave-internal kill tracking works.
    /// Currently unused — livingEnemyCount passed into Update() is sufficient.
    /// Reserved for future per-wave kill tracking if needed.
    /// </summary>
    public void ReportKill()
    {
        _waveKilledCount++;
    }

    // ── Helpers ─────────────────────────────────────────────

    private void SpawnFromWave(WaveDefinition wave)
    {
        if (!Enum.TryParse<EnemyType>(wave.EnemyType, ignoreCase: true, out var type))
            type = EnemyType.Grunt;

        float x = _rand.NextSingle() * (wave.SpawnXMax - wave.SpawnXMin) + wave.SpawnXMin;
        var pos = new Vector2(x, wave.SpawnY);
        var vel = new Vector2(wave.VelocityX, wave.VelocityY);

        _waveSpawnedCount++;
        OnSpawnEnemy?.Invoke(this, new SpawnEnemyEventArgs(type, pos, vel, wave.MovementPattern));
    }

    private void ResetWaveState()
    {
        _waveSpawnedCount = 0;
        _waveKilledCount = 0;
        _waveElapsed = 0;
        _spawnTimer = 0;
        _waveStartDelayDone = false;
        _allSpawned = false;
        _postSpawnCooldown = 0;
    }
}
