using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Level;

/// <summary>
/// Top-level object deserialized from a stage JSON file.
/// </summary>
public class LevelDefinition
{
    [JsonPropertyName("phases")]
    public List<PhaseDefinition> Phases { get; set; } = new();
}

/// <summary>
/// One phase of the level (e.g. "grunt rush", "mid-boss fight").
/// Contains ordered waves that play out sequentially.
/// </summary>
public class PhaseDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("waves")]
    public List<WaveDefinition> Waves { get; set; } = new();
}

/// <summary>
/// A single wave inside a phase. Drives enemy spawning.
/// </summary>
public class WaveDefinition
{
    /// <summary>Matches an EnemyType enum name (Grunt, BetterGrunt, MidBoss, FinalBoss).</summary>
    [JsonPropertyName("enemyType")]
    public string EnemyType { get; set; } = "Grunt";

    /// <summary>Total number of enemies to spawn in this wave.</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; } = 1;

    /// <summary>Milliseconds between each spawn.</summary>
    [JsonPropertyName("spawnIntervalMs")]
    public int SpawnIntervalMs { get; set; } = 1500;

    /// <summary>Milliseconds to wait before the first spawn of this wave.</summary>
    [JsonPropertyName("startDelayMs")]
    public int StartDelayMs { get; set; } = 0;

    /// <summary>Minimum X position for random spawn (playfield-relative).</summary>
    [JsonPropertyName("spawnXMin")]
    public float SpawnXMin { get; set; } = 60;

    /// <summary>Maximum X position for random spawn (playfield-relative).</summary>
    [JsonPropertyName("spawnXMax")]
    public float SpawnXMax { get; set; } = 900;

    /// <summary>Y position where enemies appear.</summary>
    [JsonPropertyName("spawnY")]
    public float SpawnY { get; set; } = -30;

    /// <summary>Velocity X component (pixels/sec).</summary>
    [JsonPropertyName("velocityX")]
    public float VelocityX { get; set; } = 0;

    /// <summary>Velocity Y component (pixels/sec).</summary>
    [JsonPropertyName("velocityY")]
    public float VelocityY { get; set; } = 100;

    /// <summary>
    /// How we decide this wave is "done" and the next wave can start.
    /// "timer"     – wave ends after all enemies have spawned + a cooldown.
    /// "allKilled" – wave ends only when every spawned enemy is dead.
    /// </summary>
    [JsonPropertyName("advanceCondition")]
    public string AdvanceCondition { get; set; } = "timer";

    /// <summary>
    /// Extra seconds to wait after the last spawn before advancing (used when advanceCondition == "timer").
    /// </summary>
    [JsonPropertyName("advanceCooldownSec")]
    public float AdvanceCooldownSec { get; set; } = 2.0f;

    /// <summary>
    /// Optional movement pattern identifier for future Strategy-pattern use.
    /// Current values: "linear" (default). Teammates can add "sineWave", "hover", etc.
    /// </summary>
    [JsonPropertyName("movementPattern")]
    public string MovementPattern { get; set; } = "linear";
}
