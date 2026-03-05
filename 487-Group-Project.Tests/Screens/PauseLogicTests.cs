using NUnit.Framework;
using Screens;

namespace _487_Group_Project.Tests.Screens;

[TestFixture]
public class PauseLogicTests
{
    [Test]
    public void PauseItems_HasThreeItems()
    {
        var pause = new PauseLogic();
        Assert.That(pause.Items, Has.Count.EqualTo(3));
    }

    [Test]
    public void PauseLabels_AreCorrect()
    {
        var pause = new PauseLogic();
        Assert.That(pause.Items[0].Label, Is.EqualTo("Resume"));
        Assert.That(pause.Items[1].Label, Is.EqualTo("Key Config"));
        Assert.That(pause.Items[2].Label, Is.EqualTo("Quit to Menu"));
    }

    [Test]
    public void Confirm_Resume_ReturnsResume()
    {
        var pause = new PauseLogic();
        // SelectedIndex starts at 0 which is Resume
        var action = pause.Confirm();
        Assert.That(action, Is.EqualTo("Resume"));
    }

    [Test]
    public void Confirm_QuitToMenu_ReturnsQuitToMenu()
    {
        var pause = new PauseLogic();
        pause.MoveDown(); // 1 - Key Config
        pause.MoveDown(); // 2 - Quit to Menu
        var action = pause.Confirm();
        Assert.That(action, Is.EqualTo("QuitToMenu"));
    }
}
