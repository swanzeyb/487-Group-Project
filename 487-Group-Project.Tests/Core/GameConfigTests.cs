using NUnit.Framework;
using Core;

namespace _487_Group_Project.Tests.Core;

[TestFixture]
public class GameConfigTests
{
    [Test]
    public void PlayfieldWidth_IsLessThanScreenWidth()
    {
        Assert.That(GameConfig.PlayfieldWidth, Is.LessThan(GameConfig.ScreenWidth));
    }

    [Test]
    public void PanelX_IsGreaterThanPlayfieldRight()
    {
        int playfieldRight = GameConfig.PlayfieldLeft + GameConfig.PlayfieldWidth;
        Assert.That(GameConfig.PanelX, Is.GreaterThan(playfieldRight));
    }

    [Test]
    public void PanelX_PlusPanelWidth_EqualsScreenWidth()
    {
        Assert.That(GameConfig.PanelX + GameConfig.PanelWidth, Is.EqualTo(GameConfig.ScreenWidth));
    }

    [Test]
    public void PlayfieldHeight_FitsInScreenHeight()
    {
        Assert.That(GameConfig.PlayfieldHeight, Is.LessThan(GameConfig.ScreenHeight));
    }

    [Test]
    public void PlayfieldBounds_ReturnsCorrectRectangle()
    {
        var expected = new Microsoft.Xna.Framework.Rectangle(
            GameConfig.PlayfieldLeft,
            GameConfig.PlayfieldTop,
            GameConfig.PlayfieldWidth,
            GameConfig.PlayfieldHeight);
        Assert.That(GameConfig.Playfield, Is.EqualTo(expected));
    }
}
