using NUnit.Framework;
using Screens;

namespace _487_Group_Project.Tests.Screens;

[TestFixture]
public class MenuLogicTests
{
    private MenuLogic CreateDefaultMenu()
    {
        return new MenuLogic(new List<MenuItem>
        {
            new MenuItem("Start Game", "StartGame"),
            new MenuItem("Key Config", "KeyConfig"),
            new MenuItem("Quit", "Quit")
        });
    }

    [Test]
    public void InitialSelection_IsZero()
    {
        var menu = CreateDefaultMenu();
        Assert.That(menu.SelectedIndex, Is.EqualTo(0));
    }

    [Test]
    public void MoveDown_AdvancesSelection()
    {
        var menu = CreateDefaultMenu();
        menu.MoveDown();
        Assert.That(menu.SelectedIndex, Is.EqualTo(1));
    }

    [Test]
    public void MoveUp_DecrementsSelection()
    {
        var menu = CreateDefaultMenu();
        menu.MoveDown(); // go to 1
        menu.MoveUp();   // back to 0
        Assert.That(menu.SelectedIndex, Is.EqualTo(0));
    }

    [Test]
    public void MoveDown_AtBottom_WrapsToTop()
    {
        var menu = CreateDefaultMenu();
        menu.MoveDown(); // 1
        menu.MoveDown(); // 2
        menu.MoveDown(); // wraps to 0
        Assert.That(menu.SelectedIndex, Is.EqualTo(0));
    }

    [Test]
    public void MoveUp_AtTop_WrapsToBottom()
    {
        var menu = CreateDefaultMenu();
        menu.MoveUp(); // wraps to 2
        Assert.That(menu.SelectedIndex, Is.EqualTo(2));
    }

    [Test]
    public void Confirm_ReturnsSelectedAction()
    {
        var menu = CreateDefaultMenu();
        var action = menu.Confirm();
        Assert.That(action, Is.EqualTo("StartGame"));
    }

    [Test]
    public void MenuItems_HasCorrectCount()
    {
        var menu = CreateDefaultMenu();
        Assert.That(menu.Items, Has.Count.EqualTo(3));
    }

    [Test]
    public void ItemLabels_AreCorrect()
    {
        var menu = CreateDefaultMenu();
        Assert.That(menu.Items[0].Label, Is.EqualTo("Start Game"));
        Assert.That(menu.Items[1].Label, Is.EqualTo("Key Config"));
        Assert.That(menu.Items[2].Label, Is.EqualTo("Quit"));
    }
}
