using NUnit.Framework;
using Core;

namespace _487_Group_Project.Tests.Core;

[TestFixture]
public class ScoreManagerTests
{
    [Test]
    public void InitialScore_IsZero()
    {
        var sm = new ScoreManager();
        Assert.That(sm.Score, Is.EqualTo(0));
    }

    [Test]
    public void AddScore_IncreasesScore()
    {
        var sm = new ScoreManager();
        sm.AddScore(100);
        Assert.That(sm.Score, Is.EqualTo(100));
    }

    [Test]
    public void AddScore_MultipleTimes_Accumulates()
    {
        var sm = new ScoreManager();
        sm.AddScore(50);
        sm.AddScore(30);
        Assert.That(sm.Score, Is.EqualTo(80));
    }

    [Test]
    public void AddScore_NegativeValue_DoesNotDecrease()
    {
        var sm = new ScoreManager();
        sm.AddScore(100);
        sm.AddScore(-10);
        Assert.That(sm.Score, Is.EqualTo(100));
    }

    [Test]
    public void Reset_SetsScoreToZero()
    {
        var sm = new ScoreManager();
        sm.AddScore(100);
        sm.Reset();
        Assert.That(sm.Score, Is.EqualTo(0));
    }
}
