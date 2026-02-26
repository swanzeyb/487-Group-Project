using System.Collections.Generic;

namespace Screens;

public class GameOverLogic
{
    private readonly MenuLogic _menu;
    public int FinalScore { get; }

    public GameOverLogic(int finalScore)
    {
        FinalScore = finalScore;
        _menu = new MenuLogic(new List<MenuItem>
        {
            new MenuItem("Retry", "Retry"),
            new MenuItem("Quit to Menu", "QuitToMenu")
        });
    }

    public IReadOnlyList<MenuItem> Items => _menu.Items;
    public int SelectedIndex => _menu.SelectedIndex;
    public void MoveDown() => _menu.MoveDown();
    public void MoveUp() => _menu.MoveUp();
    public string Confirm() => _menu.Confirm();
}
