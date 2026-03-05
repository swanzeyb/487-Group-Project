using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;

namespace Screens;

public class PauseScreen : IScreen
{
    private readonly PauseLogic _logic;
    private readonly SpriteFont _titleFont;
    private readonly SpriteFont _defaultFont;

    public Action OnResume;
    public Action OnKeyConfig;
    public Action OnQuitToMenu;

    public PauseScreen(SpriteFont titleFont, SpriteFont defaultFont)
    {
        _titleFont = titleFont;
        _defaultFont = defaultFont;
        _logic = new PauseLogic();
    }

    public void OnEnter() { }
    public void OnExit() { }

    public void Update(GameTime gameTime, InputState input)
    {
        if (input.Pressed(Keys.Up)) _logic.MoveUp();
        if (input.Pressed(Keys.Down)) _logic.MoveDown();

        if (input.Pressed(Keys.Escape))
        {
            OnResume?.Invoke();
            return;
        }

        if (input.Pressed(Keys.Enter))
        {
            var action = _logic.Confirm();
            switch (action)
            {
                case "Resume": OnResume?.Invoke(); break;
                case "KeyConfig": OnKeyConfig?.Invoke(); break;
                case "QuitToMenu": OnQuitToMenu?.Invoke(); break;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, SimpleDrawer drawer)
    {
        // Dark overlay
        drawer.DrawRect(spriteBatch, new Rectangle(0, 0, GameConfig.ScreenWidth, GameConfig.ScreenHeight),
            new Color(0, 0, 0, 180));

        // Title
        string title = "PAUSED";
        var titleSize = _titleFont.MeasureString(title);
        var titlePos = new Vector2((GameConfig.ScreenWidth - titleSize.X) / 2, 160);
        spriteBatch.DrawString(_titleFont, title, titlePos, Color.White);

        // Menu items
        float startY = 260;
        for (int i = 0; i < _logic.Items.Count; i++)
        {
            var item = _logic.Items[i];
            var textSize = _defaultFont.MeasureString(item.Label);
            var pos = new Vector2((GameConfig.ScreenWidth - textSize.X) / 2, startY + i * 40);
            var color = (i == _logic.SelectedIndex) ? Color.Yellow : Color.White;

            if (i == _logic.SelectedIndex)
            {
                var indicatorRect = new Rectangle((int)pos.X - 20, (int)pos.Y + 4, 10, 10);
                drawer.DrawRect(spriteBatch, indicatorRect, Color.Yellow);
            }

            spriteBatch.DrawString(_defaultFont, item.Label, pos, color);
        }
    }
}
