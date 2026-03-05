using NUnit.Framework;
using Screens;

namespace _487_Group_Project.Tests.Screens;

[TestFixture]
public class GameOverLogicTests
{
    [Test]
    public void GameOver_HasTwoItems()
    {
        var go = new GameOverLogic(finalScore: 1500);
        Assert.That(go.Items, Has.Count.EqualTo(2));
    }

    [Test]
    public void GameOver_Labels_AreCorrect()
    {
        var go = new GameOverLogic(finalScore: 1500);
        Assert.That(go.Items[0].Label, Is.EqualTo("Retry"));
        Assert.That(go.Items[1].Label, Is.EqualTo("Quit to Menu"));
    }

    [Test]
    public void GameOver_ShowsFinalScore()
    {
        var go = new GameOverLogic(finalScore: 1500);
        Assert.That(go.FinalScore, Is.EqualTo(1500));
    }

    [Test]
    public void GameOver_Confirm_Retry_ReturnsRetry()
    {
        var go = new GameOverLogic(finalScore: 1500);
        // SelectedIndex starts at 0 which is Retry
        var action = go.Confirm();
        Assert.That(action, Is.EqualTo("Retry"));
    }
}

[TestFixture]
public class VictoryLogicTests
{
    [Test]
    public void Victory_HasTwoItems()
    {
        var v = new VictoryLogic(finalScore: 3000);
        Assert.That(v.Items, Has.Count.EqualTo(2));
    }

    [Test]
    public void Victory_Labels_AreCorrect()
    {
        var v = new VictoryLogic(finalScore: 3000);
        Assert.That(v.Items[0].Label, Is.EqualTo("Retry"));
        Assert.That(v.Items[1].Label, Is.EqualTo("Quit to Menu"));
    }

    [Test]
    public void Victory_ShowsFinalScore()
    {
        var v = new VictoryLogic(finalScore: 3000);
        Assert.That(v.FinalScore, Is.EqualTo(3000));
    }

    [Test]
    public void Victory_Confirm_QuitToMenu_ReturnsQuitToMenu()
    {
        var v = new VictoryLogic(finalScore: 3000);
        v.MoveDown(); // index 1 — Quit to Menu
        var action = v.Confirm();
        Assert.That(action, Is.EqualTo("QuitToMenu"));
    }
}
