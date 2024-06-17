namespace DaLion.Core;

#region using directives

using StardewValley.Monsters;

#endregion using directive

/// <summary>The <see cref="CoreMod"/> API implementation.</summary>
public class CoreApi : ICoreApi
{
    #region status effects

    /// <inheritdoc />
    public void Bleed(Monster monster, Farmer bleeder, int duration = 30000, int stacks = 1, int maxStacks = int.MaxValue)
    {
        monster.Bleed(bleeder, duration, stacks, maxStacks);
    }

    /// <inheritdoc />
    public void Unbleed(Monster monster)
    {
        monster.Unbleed();
    }

    /// <inheritdoc />
    public bool IsBleeding(Monster monster)
    {
        return monster.IsBleeding();
    }

    /// <inheritdoc />
    public void Blind(Monster monster, int duration)
    {
        monster.Blind(duration);
    }

    /// <inheritdoc />
    public void Unblind(Monster monster)
    {
        monster.Unblind();
    }

    /// <inheritdoc />
    public bool IsBlinded(Monster monster)
    {
        return monster.IsBlinded();
    }

    /// <inheritdoc />
    public void Burn(Monster monster, Farmer burner, int duration = 15000)
    {
        monster.Burn(burner, duration);
    }

    /// <inheritdoc />
    public void Unburn(Monster monster)
    {
        monster.Unburn();
    }

    /// <inheritdoc />
    public bool IsBurning(Monster monster)
    {
        return monster.IsBurning();
    }

    /// <inheritdoc />
    public void Chill(Monster monster, int duration = 5000, float intensity = 0.5f, float freezeThreshold = 1f, bool playSoundEffect = true)
    {
        monster.Chill(duration, intensity, freezeThreshold, playSoundEffect);
    }

    /// <inheritdoc />
    public void Unchill(Monster monster)
    {
        monster.Unchill();
    }

    /// <inheritdoc />
    public bool IsChilled(Monster monster)
    {
        return monster.IsChilled();
    }

    /// <inheritdoc />
    public void Fear(Monster monster, int duration)
    {
        monster.Fear(duration);
    }

    /// <inheritdoc />
    public void Unfear(Monster monster)
    {
        monster.Unfear();
    }

    /// <inheritdoc />
    public bool IsFeared(Monster monster)
    {
        return monster.IsFeared();
    }

    /// <inheritdoc />
    public void Freeze(Monster monster, int duration = 30000)
    {
        monster.Freeze(duration);
    }

    /// <inheritdoc />
    public void Defrost(Monster monster)
    {
        monster.Defrost();
    }

    /// <inheritdoc />
    public bool IsFrozen(Monster monster)
    {
        return monster.IsFrozen();
    }

    /// <inheritdoc />
    public void Poison(Monster monster, Farmer poisoner, int duration = 15000, int stacks = 1, int maxStacks = int.MaxValue)
    {
        monster.Poison(poisoner, duration, stacks, maxStacks);
    }

    /// <inheritdoc />
    public void Detox(Monster monster)
    {
        monster.Detox();
    }

    /// <inheritdoc />
    public bool IsPoisoned(Monster monster)
    {
        return monster.IsPoisoned();
    }

    /// <inheritdoc />
    public void Slow(Monster monster, int duration, float intensity = 0.5f)
    {
        monster.Slow(duration, intensity);
    }

    /// <inheritdoc />
    public void Unslow(Monster monster)
    {
        monster.Unslow();
    }

    /// <inheritdoc />
    public bool IsSlowed(Monster monster)
    {
        return monster.IsSlowed();
    }

    /// <inheritdoc />
    public void Stun(Monster monster, int duration)
    {
        monster.Stun(duration);
    }

    /// <inheritdoc />
    public bool IsStunned(Monster monster)
    {
        return monster.IsStunned();
    }

    #endregion status effects
}
