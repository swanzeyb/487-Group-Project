using Core;
using Microsoft.Xna.Framework;

namespace Entities;

/// <summary>
/// Centralised factory for creating Enemy instances.
/// Teammates adding HP, bullet patterns, etc. only need to modify this one place.
/// </summary>
public static class EnemyFactory
{
    public static Enemy Create(SimpleDrawer drawer, EnemyType type, Vector2 position, Vector2 velocity)
    {
        return new Enemy(drawer, type, position, velocity);
    }
}
