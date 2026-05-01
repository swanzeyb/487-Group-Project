using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;
using Entities;

namespace Screens;

public class TestSelectionScreen : IScreen
{
    private readonly TestSelectionLogic _logic = new();
    private SpriteFont _titleFont;
    private SpriteFont _defaultFont;

    public Action OnEnterGame;

    public TestSelectionScreen(SpriteFont titleFont, SpriteFont defaultFont)
    {
        _titleFont   = titleFont;
        _defaultFont = defaultFont;
    }

    public void OnEnter() { }
    public void OnExit()  { }

    public void Update(GameTime gameTime, InputState input)
    {
        if (input.Pressed(Keys.Up))    _logic.MoveUp();
        if (input.Pressed(Keys.Down))  _logic.MoveDown();
        if (input.Pressed(Keys.Left))  _logic.CycleLeft();
        if (input.Pressed(Keys.Right)) _logic.CycleRight();

        if (input.Pressed(Keys.Enter) && _logic.Confirm())
        {
            GameConfig.SelectedEnemyType = _logic.SelectedEnemyType switch
            {
                "BetterGrunt" => EnemyType.BetterGrunt,
                "MidBoss"     => EnemyType.MidBoss,
                "FinalBoss"   => EnemyType.FinalBoss,
                _             => EnemyType.Grunt
            };
            GameConfig.SelectedMovementPattern = _logic.SelectedMovement;
            GameConfig.SelectedAttackPattern   = _logic.SelectedAttack == "default" ? "" : _logic.SelectedAttack;
            GameConfig.IsDebugMode = true;
            OnEnterGame?.Invoke();
        }
    }

    public void Draw(SpriteBatch spriteBatch, SimpleDrawer drawer)
    {
        // Title
        string title = "TEST MODE";
        var titleSize = _titleFont.MeasureString(title);
        spriteBatch.DrawString(_titleFont, title,
            new Vector2((GameConfig.ScreenWidth - titleSize.X) / 2, 80), Color.Red);

        // Subtitle / instructions
        string hint = "Up/Down: row   Left/Right: change value   Enter: start";
        var hintSize = _defaultFont.MeasureString(hint);
        spriteBatch.DrawString(_defaultFont, hint,
            new Vector2((GameConfig.ScreenWidth - hintSize.X) / 2, 145), Color.Gray);

        float startY = 210;
        DrawRow(spriteBatch, drawer, 0, startY,       "Enemy Type",     _logic.SelectedEnemyType);
        DrawRow(spriteBatch, drawer, 1, startY + 60,  "Movement",       _logic.SelectedMovement);
        DrawRow(spriteBatch, drawer, 2, startY + 120, "Attack Pattern", _logic.SelectedAttack);

        // Enter Game row
        float enterY = startY + 210;
        bool enterFocused = _logic.FocusedRow == 3;
        string enterText = "[ Start Test ]";
        var enterSize = _defaultFont.MeasureString(enterText);
        var enterPos  = new Vector2((GameConfig.ScreenWidth - enterSize.X) / 2, enterY);
        if (enterFocused)
            drawer.DrawRect(spriteBatch,
                new Rectangle((int)enterPos.X - 20, (int)enterPos.Y + 4, 10, 10), Color.Yellow);
        spriteBatch.DrawString(_defaultFont, enterText, enterPos, enterFocused ? Color.Yellow : Color.White);
    }

    private void DrawRow(SpriteBatch spriteBatch, SimpleDrawer drawer,
                         int rowIndex, float y, string label, string value)
    {
        bool focused = _logic.FocusedRow == rowIndex;
        float cx = GameConfig.ScreenWidth / 2f;

        // Focus dot
        if (focused)
            drawer.DrawRect(spriteBatch,
                new Rectangle((int)(cx - 220), (int)y + 4, 10, 10), Color.Yellow);

        // Label
        spriteBatch.DrawString(_defaultFont, label + ":",
            new Vector2(cx - 200, y), focused ? Color.Yellow : Color.LightGray);

        // Value with arrows
        string display = $"< {value} >";
        var displaySize = _defaultFont.MeasureString(display);
        spriteBatch.DrawString(_defaultFont, display,
            new Vector2(cx - displaySize.X / 2 + 80, y), focused ? Color.Cyan : Color.White);
    }
}
