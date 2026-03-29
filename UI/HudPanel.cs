using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core;

namespace UI;

public class HudPanel
{
    private readonly SpriteFont _font;
    private readonly SimpleDrawer _drawer;

    public HudPanel(SpriteFont font, SimpleDrawer drawer)
    {
        _font = font;
        _drawer = drawer;
    }

    public void Draw(SpriteBatch spriteBatch, HudPanelData data)
    {
        int panelX = GameConfig.PanelX;
        int panelW = GameConfig.PanelWidth;
        int panelY = GameConfig.PlayfieldTop;
        int panelH = GameConfig.PlayfieldHeight;

        // Panel background
        _drawer.DrawRect(spriteBatch, new Rectangle(panelX, panelY, panelW, panelH),
            new Color(20, 20, 30, 220));
        _drawer.DrawRectOutline(spriteBatch, new Rectangle(panelX, panelY, panelW, panelH),
            2, Color.DarkGray);

        float x = panelX + 16;
        float y = panelY + 16;
        float lineHeight = 28;

        // Player label
        spriteBatch.DrawString(_font, "Player", new Vector2(x, y), Color.White);
        y += lineHeight + 4;

        // Lives
        spriteBatch.DrawString(_font, $"Lives:", new Vector2(x, y), Color.LightGray);
        for (int i = 0; i < data.Lives; i++)
        {
            float starX = x + 60 + i * 20;
            _drawer.DrawRect(spriteBatch, new Rectangle((int)starX, (int)y + 2, 12, 12), Color.Gold);
        }
        y += lineHeight;

        // --- Player HP Bar ---
        spriteBatch.DrawString(_font, "HP:", new Vector2(x, y), Color.LightGray);

        int hpBarWidth = panelW - 80;
        int hpBarHeight = 16;
        int hpBarX = (int)x + 40;
        int hpBarY = (int)y + 6;

        // Calculate percentage (clamped to prevent drawing errors)
        float healthPercent = data.PlayerMaxHealth > 0 ? (float)data.PlayerHealth / data.PlayerMaxHealth : 0f;
        healthPercent = Math.Clamp(healthPercent, 0f, 1f);

        // HP Background (Red)
        _drawer.DrawRect(spriteBatch, new Rectangle(hpBarX, hpBarY, hpBarWidth, hpBarHeight), Color.DarkRed);

        // HP Fill
        int filledWidth = (int)(hpBarWidth * healthPercent);
        if (filledWidth > 0)
        {
            // Dynamic coloring based on health percentage
            Color hpColor = healthPercent > 0.5f ? Color.Green : (healthPercent > 0.25f ? Color.Orange : Color.Red);
            _drawer.DrawRect(spriteBatch, new Rectangle(hpBarX, hpBarY, filledWidth, hpBarHeight), hpColor);
        }

        // HP Outline
        _drawer.DrawRectOutline(spriteBatch, new Rectangle(hpBarX, hpBarY, hpBarWidth, hpBarHeight), 1, Color.White);
        y += lineHeight;
        // -----------------------

        // Score
        spriteBatch.DrawString(_font, $"Score: {data.Score}", new Vector2(x, y), Color.LightGray);
        y += lineHeight;

        // Bombs
        spriteBatch.DrawString(_font, $"Bombs: {data.BombCount}", new Vector2(x, y), Color.LightGray);
        y += lineHeight + 16;

        // Separator
        _drawer.DrawRect(spriteBatch, new Rectangle(panelX + 10, (int)y, panelW - 20, 1), Color.DarkGray);
        y += 12;

        // Phase
        spriteBatch.DrawString(_font, "Phase:", new Vector2(x, y), Color.White);
        y += lineHeight;
        string phaseName = string.IsNullOrEmpty(data.PhaseName) ? "---" : data.PhaseName;
        spriteBatch.DrawString(_font, phaseName, new Vector2(x + 8, y), Color.Cyan);
        y += lineHeight + 16;

        // Boss HP bar
        if (data.BossHealthPercent < 1f || (data.PhaseName?.ToLower().Contains("boss") ?? false))
        {
            spriteBatch.DrawString(_font, "Boss HP:", new Vector2(x, y), Color.White);
            y += lineHeight;

            int barWidth = panelW - 40;
            int barHeight = 16;
            int barX = panelX + 20;

            // Background bar
            _drawer.DrawRect(spriteBatch, new Rectangle(barX, (int)y, barWidth, barHeight), Color.DarkRed);
            // Health bar
            int healthWidth = (int)(barWidth * data.BossHealthPercent);
            if (healthWidth > 0)
                _drawer.DrawRect(spriteBatch, new Rectangle(barX, (int)y, healthWidth, barHeight), Color.Red);
            // Outline
            _drawer.DrawRectOutline(spriteBatch, new Rectangle(barX, (int)y, barWidth, barHeight), 1, Color.White);
        }
    }
}