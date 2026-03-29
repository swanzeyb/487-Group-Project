using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;
using Entities;

namespace Screens;

public class TestSelectionScreen : IScreen
{
    private readonly TestSelectionLogic _logic;
    private SpriteFont _titleFont;
    private SpriteFont _defaultFont;

    public Action OnEnterGame;

    public TestSelectionScreen(SpriteFont titleFont, SpriteFont defaultFont)
    {
        _titleFont = titleFont;
        _defaultFont = defaultFont;
        _logic = new TestSelectionLogic(new List<MenuItem>
        {
            new MenuItem("Grunt", "Grunt"),
            new MenuItem("Better Grunt", "BetterGrunt"),
            new MenuItem("Mid Boss", "MidBoss"),
            new MenuItem("Final Boss", "FinalBoss"),
            new MenuItem("Enter Game", "EnterGame")
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
                case "Grunt": GameConfig.SelectedEnemyType = EnemyType.Grunt; break;
                case "BetterGrunt": GameConfig.SelectedEnemyType = EnemyType.BetterGrunt; break;
                case "MidBoss": GameConfig.SelectedEnemyType = EnemyType.MidBoss; break;
                case "FinalBoss": GameConfig.SelectedEnemyType = EnemyType.FinalBoss; break;
                case "EnterGame":
                    GameConfig.IsDebugMode = true;
                    OnEnterGame?.Invoke();
                    break;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, SimpleDrawer drawer)
    {
        // Title
        string title = "TEST MODE";
        var titleSize = _titleFont.MeasureString(title);
        var titlePos = new Vector2((GameConfig.ScreenWidth - titleSize.X) / 2, 120);
        spriteBatch.DrawString(_titleFont, title, titlePos, Color.Red);

        // Subtitle
        string subtitle = "Select Enemy Type";
        var subSize = _defaultFont.MeasureString(subtitle);
        var subPos = new Vector2((GameConfig.ScreenWidth - subSize.X) / 2, 180);
        spriteBatch.DrawString(_defaultFont, subtitle, subPos, Color.Yellow);

        // Menu items
        float startY = 220;
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

        // Selected enemy type display
        string selectedEnemy = $"Selected Enemy: {GameConfig.SelectedEnemyType}";
        var selectedSize = _defaultFont.MeasureString(selectedEnemy);
        var selectedPos = new Vector2((GameConfig.ScreenWidth - selectedSize.X) / 2, 410);
        spriteBatch.DrawString(_defaultFont, selectedEnemy, selectedPos, Color.Cyan);

        // Instructions
        string instructions = "Arrow Keys to navigate, Enter to select";
        var instrSize = _defaultFont.MeasureString(instructions);
        var instrPos = new Vector2((GameConfig.ScreenWidth - instrSize.X) / 2, 440);
        spriteBatch.DrawString(_defaultFont, instructions, instrPos, Color.Gray);
    }
}