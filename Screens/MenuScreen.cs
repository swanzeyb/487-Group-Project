using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;

namespace Screens;

public class MenuScreen : IScreen
{
    private readonly MenuLogic _logic;
    private SpriteFont _titleFont;
    private SpriteFont _defaultFont;

    public Action OnStartGame;
    public Action OnKeyConfig;
    public Action OnQuit;

    public MenuScreen(SpriteFont titleFont, SpriteFont defaultFont)
    {
        _titleFont = titleFont;
        _defaultFont = defaultFont;
        _logic = new MenuLogic(new List<MenuItem>
        {
            new MenuItem("Start Game", "StartGame"),
            new MenuItem("Key Config", "KeyConfig"),
            new MenuItem("Quit", "Quit")
        });
    }

    public void OnEnter() { }
    public void OnExit() { }

    public void Update(GameTime gameTime, InputState input)
    {
        if (input.Pressed(Keys.Up)) _logic.MoveUp();
        if (input.Pressed(Keys.Down)) _logic.MoveDown();

        if (input.Pressed(Keys.Enter))
        {
            var action = _logic.Confirm();
            switch (action)
            {
                case "StartGame": OnStartGame?.Invoke(); break;
                case "KeyConfig": OnKeyConfig?.Invoke(); break;
                case "Quit": OnQuit?.Invoke(); break;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, SimpleDrawer drawer)
    {
        // Title
        string title = "BULLET HELL";
        var titleSize = _titleFont.MeasureString(title);
        var titlePos = new Vector2((GameConfig.ScreenWidth - titleSize.X) / 2, 120);
        spriteBatch.DrawString(_titleFont, title, titlePos, Color.Red);

        // Menu items
        float startY = 260;
        for (int i = 0; i < _logic.Items.Count; i++)
        {
            var item = _logic.Items[i];
            var text = item.Label;
            var textSize = _defaultFont.MeasureString(text);
            var pos = new Vector2((GameConfig.ScreenWidth - textSize.X) / 2, startY + i * 40);
            var color = (i == _logic.SelectedIndex) ? Color.Yellow : Color.White;

            if (i == _logic.SelectedIndex)
            {
                // Draw selection indicator
                var indicatorRect = new Rectangle((int)pos.X - 20, (int)pos.Y + 4, 10, 10);
                drawer.DrawRect(spriteBatch, indicatorRect, Color.Yellow);
            }

            spriteBatch.DrawString(_defaultFont, text, pos, color);
        }

        // Instructions
        string instructions = "Arrow Keys to navigate, Enter to select";
        var instrSize = _defaultFont.MeasureString(instructions);
        var instrPos = new Vector2((GameConfig.ScreenWidth - instrSize.X) / 2, 440);
        spriteBatch.DrawString(_defaultFont, instructions, instrPos, Color.Gray);
    }
}
