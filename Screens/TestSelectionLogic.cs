using System.Collections.Generic;

namespace Screens;

public class TestSelectionLogic
{
    public IReadOnlyList<MenuItem> Items { get; }
    public int SelectedIndex { get; private set; }

    public TestSelectionLogic(List<MenuItem> items)
    {
        Items = items.AsReadOnly();
        SelectedIndex = 0;
    }

    public void MoveDown()
    {
        SelectedIndex = (SelectedIndex + 1) % Items.Count;
    }

    public void MoveUp()
    {
        SelectedIndex = (SelectedIndex - 1 + Items.Count) % Items.Count;
    }

    public string Confirm()
    {
        return Items[SelectedIndex].Action;
    }
}