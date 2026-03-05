using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;

namespace Screens;

public class KeyConfigScreen : IScreen
{
    private readonly KeyBindings _keyBindings;
    private readonly SpriteFont _titleFont;
    private readonly SpriteFont _defaultFont;
    private int _selectedIndex = 0;
    private bool _listening = false;

    public Action OnBack;

    // AllActions + 2 extra entries: "Reset Defaults" and "Back"
    private int TotalItems => _keyBindings.AllActions.Count + 2;

    public KeyConfigScreen(KeyBindings keyBindings, SpriteFont titleFont, SpriteFont defaultFont)
    {
        _keyBindings = keyBindings;
        _titleFont = titleFont;
        _defaultFont = defaultFont;
    }

    public void OnEnter()
    {
        _selectedIndex = 0;
        _listening = false;
    }

    public void OnExit() { }

    public void Update(GameTime gameTime, InputState input)
    {
        if (_listening)
        {
            // Wait for any key press
            var keys = input.Current.GetPressedKeys();
            if (keys.Length > 0 && !input.Previous.IsKeyDown(keys[0]))
            {
                var action = _keyBindings.AllActions[_selectedIndex];
                _keyBindings.Rebind(action, keys[0]);
                _listening = false;
            }
            return;
        }

        if (input.Pressed(Keys.Escape))
        {
            OnBack?.Invoke();
            return;
        }

        if (input.Pressed(Keys.Up))
            _selectedIndex = (_selectedIndex - 1 + TotalItems) % TotalItems;
        if (input.Pressed(Keys.Down))
            _selectedIndex = (_selectedIndex + 1) % TotalItems;

        if (input.Pressed(Keys.Enter))
        {
            int actionCount = _keyBindings.AllActions.Count;
            if (_selectedIndex < actionCount)
            {
                // Start listening for a new key
                _listening = true;
            }
            else if (_selectedIndex == actionCount)
            {
                // Reset Defaults
                _keyBindings.ResetToDefaults();
            }
            else if (_selectedIndex == actionCount + 1)
            {
                // Back
                OnBack?.Invoke();
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, SimpleDrawer drawer)
    {
        // Dark background
        drawer.DrawRect(spriteBatch, new Rectangle(0, 0, GameConfig.ScreenWidth, GameConfig.ScreenHeight),
            new Color(0, 0, 30, 240));

        // Title
        string title = "KEY CONFIG";
        var titleSize = _titleFont.MeasureString(title);
        spriteBatch.DrawString(_titleFont, title,
            new Vector2((GameConfig.ScreenWidth - titleSize.X) / 2, 40), Color.White);

        // Key bindings list
        float startY = 100;
        float lineHeight = 30;
        int actionCount = _keyBindings.AllActions.Count;

        for (int i = 0; i < actionCount; i++)
        {
            var action = _keyBindings.AllActions[i];
            var key = _keyBindings.GetKey(action);
            var color = (i == _selectedIndex) ? Color.Yellow : Color.White;

            string display;
            if (_listening && i == _selectedIndex)
                display = $"{action}: [Press a key...]";
            else
                display = $"{action}: {key}";

            var pos = new Vector2(300, startY + i * lineHeight);

            if (i == _selectedIndex)
            {
                var indicatorRect = new Rectangle((int)pos.X - 16, (int)pos.Y + 5, 8, 8);
                drawer.DrawRect(spriteBatch, indicatorRect, Color.Yellow);
            }

            spriteBatch.DrawString(_defaultFont, display, pos, color);
        }

        // Reset Defaults option
        float resetY = startY + actionCount * lineHeight + 20;
        var resetColor = (_selectedIndex == actionCount) ? Color.Yellow : Color.LightGray;
        var resetPos = new Vector2(300, resetY);
        if (_selectedIndex == actionCount)
        {
            var indicatorRect = new Rectangle((int)resetPos.X - 16, (int)resetPos.Y + 5, 8, 8);
            drawer.DrawRect(spriteBatch, indicatorRect, Color.Yellow);
        }
        spriteBatch.DrawString(_defaultFont, "Reset Defaults", resetPos, resetColor);

        // Back option
        float backY = resetY + lineHeight;
        var backColor = (_selectedIndex == actionCount + 1) ? Color.Yellow : Color.LightGray;
        var backPos = new Vector2(300, backY);
        if (_selectedIndex == actionCount + 1)
        {
            var indicatorRect = new Rectangle((int)backPos.X - 16, (int)backPos.Y + 5, 8, 8);
            drawer.DrawRect(spriteBatch, indicatorRect, Color.Yellow);
        }
        spriteBatch.DrawString(_defaultFont, "Back", backPos, backColor);

        // Instructions
        string instructions = "Enter to select/rebind, Escape to go back";
        var instrSize = _defaultFont.MeasureString(instructions);
        spriteBatch.DrawString(_defaultFont, instructions,
            new Vector2((GameConfig.ScreenWidth - instrSize.X) / 2, GameConfig.ScreenHeight - 40), Color.Gray);
    }
}
