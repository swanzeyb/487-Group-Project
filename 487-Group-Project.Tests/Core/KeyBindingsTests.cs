using NUnit.Framework;
using Core;
using Microsoft.Xna.Framework.Input;

namespace _487_Group_Project.Tests.Core;

[TestFixture]
public class KeyBindingsTests
{
    [Test]
    public void DefaultBindings_ContainsAllActions()
    {
        var kb = new KeyBindings();
        var actions = kb.AllActions;
        Assert.That(actions, Does.Contain("MoveUp"));
        Assert.That(actions, Does.Contain("MoveDown"));
        Assert.That(actions, Does.Contain("MoveLeft"));
        Assert.That(actions, Does.Contain("MoveRight"));
        Assert.That(actions, Does.Contain("Shoot"));
        Assert.That(actions, Does.Contain("Slow"));
        Assert.That(actions, Does.Contain("Bomb"));
        Assert.That(actions, Does.Contain("Pause"));
    }

    [Test]
    public void DefaultBindings_MoveUp_IsW()
    {
        var kb = new KeyBindings();
        Assert.That(kb.GetKey("MoveUp"), Is.EqualTo(Keys.W));
    }

    [Test]
    public void DefaultBindings_MoveDown_IsS()
    {
        var kb = new KeyBindings();
        Assert.That(kb.GetKey("MoveDown"), Is.EqualTo(Keys.S));
    }

    [Test]
    public void DefaultBindings_MoveLeft_IsA()
    {
        var kb = new KeyBindings();
        Assert.That(kb.GetKey("MoveLeft"), Is.EqualTo(Keys.A));
    }

    [Test]
    public void DefaultBindings_MoveRight_IsD()
    {
        var kb = new KeyBindings();
        Assert.That(kb.GetKey("MoveRight"), Is.EqualTo(Keys.D));
    }

    [Test]
    public void DefaultBindings_Shoot_IsSpace()
    {
        var kb = new KeyBindings();
        Assert.That(kb.GetKey("Shoot"), Is.EqualTo(Keys.Space));
    }

    [Test]
    public void DefaultBindings_Slow_IsLeftShift()
    {
        var kb = new KeyBindings();
        Assert.That(kb.GetKey("Slow"), Is.EqualTo(Keys.LeftShift));
    }

    [Test]
    public void DefaultBindings_Pause_IsEscape()
    {
        var kb = new KeyBindings();
        Assert.That(kb.GetKey("Pause"), Is.EqualTo(Keys.Escape));
    }

    [Test]
    public void Rebind_ChangesKey()
    {
        var kb = new KeyBindings();
        kb.Rebind("MoveUp", Keys.I);
        Assert.That(kb.GetKey("MoveUp"), Is.EqualTo(Keys.I));
    }

    [Test]
    public void Rebind_DoesNotAffectOtherActions()
    {
        var kb = new KeyBindings();
        kb.Rebind("MoveUp", Keys.I);
        Assert.That(kb.GetKey("MoveDown"), Is.EqualTo(Keys.S));
    }

    [Test]
    public void GetKey_UnknownAction_ThrowsKeyNotFoundException()
    {
        var kb = new KeyBindings();
        Assert.That(() => kb.GetKey("NonexistentAction"),
            Throws.TypeOf<KeyNotFoundException>());
    }

    [Test]
    public void ResetToDefaults_RestoresOriginalBindings()
    {
        var kb = new KeyBindings();
        kb.Rebind("MoveUp", Keys.I);
        kb.ResetToDefaults();
        Assert.That(kb.GetKey("MoveUp"), Is.EqualTo(Keys.W));
    }
}
