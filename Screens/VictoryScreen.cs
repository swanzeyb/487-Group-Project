using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;

namespace Screens;

public class VictoryScreen : IScreen
{
    private VictoryLogic _logic;
    private readonly SpriteFont _titleFont;
    private readonly SpriteFont _defaultFont;

    public Action OnRetry;
    public Action OnQuitToMenu;

    public VictoryScreen(SpriteFont titleFont, SpriteFont defaultFont)
    {
        _titleFont = titleFont;
        _defaultFont = defaultFont;
        _logic = new VictoryLogic(0);
    }

    public void SetScore(int score)
    {
        _logic = new VictoryLogic(score);
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
                case "Retry": OnRetry?.Invoke(); break;
                case "QuitToMenu": OnQuitToMenu?.Invoke(); break;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, SimpleDrawer drawer)
    {
        // Dark background
        drawer.DrawRect(spriteBatch, new Rectangle(0, 0, GameConfig.ScreenWidth, GameConfig.ScreenHeight),
            new Color(0, 20, 0, 240));

        // Title
        string title = "VICTORY!";
        var titleSize = _titleFont.MeasureString(title);
        var titlePos = new Vector2((GameConfig.ScreenWidth - titleSize.X) / 2, 140);
        spriteBatch.DrawString(_titleFont, title, titlePos, Color.Gold);

        // Score
        string scoreText = $"Final Score: {_logic.FinalScore}";
        var scoreSize = _defaultFont.MeasureString(scoreText);
        var scorePos = new Vector2((GameConfig.ScreenWidth - scoreSize.X) / 2, 220);
        spriteBatch.DrawString(_defaultFont, scoreText, scorePos, Color.White);

        // Menu items
        float startY = 300;
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
