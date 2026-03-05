using NUnit.Framework;
using Core;
using Screens;

namespace _487_Group_Project.Tests.Core;

[TestFixture]
public class GameStateEnumTests
{
    [Test]
    public void GameState_HasMenuValue()
    {
        var state = GameState.Menu;
        Assert.That(state, Is.EqualTo(GameState.Menu));
    }

    [Test]
    public void GameState_HasPlayingValue()
    {
        var state = GameState.Playing;
        Assert.That(state, Is.EqualTo(GameState.Playing));
    }

    [Test]
    public void GameState_HasPausedValue()
    {
        var state = GameState.Paused;
        Assert.That(state, Is.EqualTo(GameState.Paused));
    }

    [Test]
    public void GameState_HasGameOverValue()
    {
        var state = GameState.GameOver;
        Assert.That(state, Is.EqualTo(GameState.GameOver));
    }

    [Test]
    public void GameState_HasVictoryValue()
    {
        var state = GameState.Victory;
        Assert.That(state, Is.EqualTo(GameState.Victory));
    }
}

[TestFixture]
public class ScreenManagerTests
{
    [Test]
    public void InitialState_IsMenu()
    {
        var sm = new ScreenManager();
        Assert.That(sm.CurrentState, Is.EqualTo(GameState.Menu));
    }

    [Test]
    public void TransitionTo_Playing_ChangesState()
    {
        var sm = new ScreenManager();
        var result = sm.TransitionTo(GameState.Playing);
        Assert.That(result, Is.True);
        Assert.That(sm.CurrentState, Is.EqualTo(GameState.Playing));
    }

    [Test]
    public void TransitionTo_Paused_FromPlaying_Succeeds()
    {
        var sm = new ScreenManager();
        sm.TransitionTo(GameState.Playing);
        var result = sm.TransitionTo(GameState.Paused);
        Assert.That(result, Is.True);
        Assert.That(sm.CurrentState, Is.EqualTo(GameState.Paused));
    }

    [Test]
    public void TransitionTo_Paused_FromMenu_IsIgnored()
    {
        var sm = new ScreenManager();
        var result = sm.TransitionTo(GameState.Paused);
        Assert.That(result, Is.False);
        Assert.That(sm.CurrentState, Is.EqualTo(GameState.Menu));
    }

    [Test]
    public void TransitionTo_GameOver_FromPlaying_Succeeds()
    {
        var sm = new ScreenManager();
        sm.TransitionTo(GameState.Playing);
        var result = sm.TransitionTo(GameState.GameOver);
        Assert.That(result, Is.True);
        Assert.That(sm.CurrentState, Is.EqualTo(GameState.GameOver));
    }

    [Test]
    public void TransitionTo_Victory_FromPlaying_Succeeds()
    {
        var sm = new ScreenManager();
        sm.TransitionTo(GameState.Playing);
        var result = sm.TransitionTo(GameState.Victory);
        Assert.That(result, Is.True);
        Assert.That(sm.CurrentState, Is.EqualTo(GameState.Victory));
    }

    [Test]
    public void TransitionTo_Menu_FromGameOver_Succeeds()
    {
        var sm = new ScreenManager();
        sm.TransitionTo(GameState.Playing);
        sm.TransitionTo(GameState.GameOver);
        var result = sm.TransitionTo(GameState.Menu);
        Assert.That(result, Is.True);
        Assert.That(sm.CurrentState, Is.EqualTo(GameState.Menu));
    }

    [Test]
    public void TransitionTo_Menu_FromVictory_Succeeds()
    {
        var sm = new ScreenManager();
        sm.TransitionTo(GameState.Playing);
        sm.TransitionTo(GameState.Victory);
        var result = sm.TransitionTo(GameState.Menu);
        Assert.That(result, Is.True);
        Assert.That(sm.CurrentState, Is.EqualTo(GameState.Menu));
    }

    [Test]
    public void TransitionTo_Playing_FromPaused_Succeeds()
    {
        var sm = new ScreenManager();
        sm.TransitionTo(GameState.Playing);
        sm.TransitionTo(GameState.Paused);
        var result = sm.TransitionTo(GameState.Playing);
        Assert.That(result, Is.True);
        Assert.That(sm.CurrentState, Is.EqualTo(GameState.Playing));
    }
}
