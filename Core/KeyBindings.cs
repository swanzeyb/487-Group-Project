using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Core;

public class KeyBindings
{
    private readonly Dictionary<string, Keys> _defaults;
    private readonly Dictionary<string, Keys> _bindings;

    public KeyBindings()
    {
        _defaults = new Dictionary<string, Keys>
        {
            { "MoveUp", Keys.W },
            { "MoveDown", Keys.S },
            { "MoveLeft", Keys.A },
            { "MoveRight", Keys.D },
            { "Shoot", Keys.Space },
            { "Slow", Keys.LeftShift },
            { "Bomb", Keys.B },
            { "Pause", Keys.Escape }
        };
        _bindings = new Dictionary<string, Keys>(_defaults);
    }

    public IReadOnlyList<string> AllActions => _bindings.Keys.ToList().AsReadOnly();

    public Keys GetKey(string action)
    {
        if (!_bindings.ContainsKey(action))
            throw new KeyNotFoundException($"Unknown action: {action}");
        return _bindings[action];
    }

    public void Rebind(string action, Keys key)
    {
        if (!_bindings.ContainsKey(action))
            throw new KeyNotFoundException($"Unknown action: {action}");
        _bindings[action] = key;
    }

    public void ResetToDefaults()
    {
        _bindings.Clear();
        foreach (var kvp in _defaults)
            _bindings[kvp.Key] = kvp.Value;
    }
}
