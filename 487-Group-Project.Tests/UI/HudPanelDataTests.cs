using NUnit.Framework;
using UI;

namespace _487_Group_Project.Tests.UI;

[TestFixture]
public class HudPanelDataTests
{
    [Test]
    public void DefaultLives_IsThree()
    {
        var hud = new HudPanelData();
        Assert.That(hud.Lives, Is.EqualTo(3));
    }

    [Test]
    public void DefaultScore_IsZero()
    {
        var hud = new HudPanelData();
        Assert.That(hud.Score, Is.EqualTo(0));
    }

    [Test]
    public void DefaultPhaseName_IsEmpty()
    {
        var hud = new HudPanelData();
        Assert.That(hud.PhaseName, Is.EqualTo(""));
    }

    [Test]
    public void DefaultBossHealthPercent_IsOne()
    {
        var hud = new HudPanelData();
        Assert.That(hud.BossHealthPercent, Is.EqualTo(1f));
    }

    [Test]
    public void DefaultBombCount_IsZero()
    {
        var hud = new HudPanelData();
        Assert.That(hud.BombCount, Is.EqualTo(0));
    }

    [Test]
    public void BossHealthPercent_ClampedToZero()
    {
        var hud = new HudPanelData();
        hud.BossHealthPercent = -0.5f;
        Assert.That(hud.BossHealthPercent, Is.EqualTo(0f));
    }

    [Test]
    public void BossHealthPercent_ClampedToOne()
    {
        var hud = new HudPanelData();
        hud.BossHealthPercent = 1.5f;
        Assert.That(hud.BossHealthPercent, Is.EqualTo(1f));
    }
}
