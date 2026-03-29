using System.Collections.Generic;
using Core;

namespace Screens;

public class ScreenManager
{
    public GameState CurrentState { get; private set; } = GameState.Menu;

    private static readonly Dictionary<GameState, HashSet<GameState>> _validTransitions = new()
    {
        { GameState.Menu, new HashSet<GameState> { GameState.TestSelection, GameState.Playing } },
        { GameState.TestSelection, new HashSet<GameState> { GameState.Playing } },
        { GameState.Playing, new HashSet<GameState> { GameState.Paused, GameState.GameOver, GameState.Victory } },
        { GameState.Paused, new HashSet<GameState> { GameState.Playing, GameState.Menu } },
        { GameState.GameOver, new HashSet<GameState> { GameState.Menu, GameState.Playing } },
        { GameState.Victory, new HashSet<GameState> { GameState.Menu, GameState.Playing } },
    };

    public bool TransitionTo(GameState newState)
    {
        if (_validTransitions.TryGetValue(CurrentState, out var valid) && valid.Contains(newState))
        {
            CurrentState = newState;
            return true;
        }
        return false;
    }
}
