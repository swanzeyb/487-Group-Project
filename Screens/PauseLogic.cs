using System.Collections.Generic;

namespace Screens;

public class PauseLogic
{
    private readonly MenuLogic _menu;

    public PauseLogic()
    {
        _menu = new MenuLogic(new List<MenuItem>
        {
            new MenuItem("Resume", "Resume"),
            new MenuItem("Key Config", "KeyConfig"),
            new MenuItem("Quit to Menu", "QuitToMenu")
        });
    }

    public IReadOnlyList<MenuItem> Items => _menu.Items;
    public int SelectedIndex => _menu.SelectedIndex;
    public void MoveDown() => _menu.MoveDown();
    public void MoveUp() => _menu.MoveUp();
    public string Confirm() => _menu.Confirm();
}
