namespace DaLion.Ligo.Modules.Professions.Ultimates;

#region using directives

using StardewValley.Monsters;

#endregion using directives

/// <summary>A <see cref="GreenSlime"/> under influence of <see cref="Concerto"/>.</summary>
internal sealed class PipedSlime
{
    /// <summary>Initializes a new instance of the <see cref="PipedSlime"/> class.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/> instance.</param>
    /// <param name="piper">The <see cref="Farmer"/> who cast <see cref="Concerto"/>.</param>
    internal PipedSlime(GreenSlime slime, Farmer piper)
    {
        this.Instance = slime;
        this.Piper = piper;
        this.PipeTimer = (int)(30000 / ProfessionsModule.Config.SpecialDrainFactor);
        this.OriginalHealth = slime.MaxHealth;
        this.OriginalRange = slime.moveTowardPlayerThreshold.Value;
        this.OriginalScale = slime.Scale;
        this.FakeFarmer = new FakeFarmer
        {
            UniqueMultiplayerID = slime.GetHashCode(), currentLocation = slime.currentLocation,
        };
    }

    /// <summary>Gets the <see cref="GreenSlime"/> instance.</summary>
    internal GreenSlime Instance { get; }

    /// <summary>Gets the <see cref="Farmer"/> who cast <see cref="Concerto"/>.</summary>
    internal Farmer Piper { get; }

    /// <summary>Gets or sets the time left on the <see cref="Concerto"/> effect.</summary>
    internal int PipeTimer { get; set; }

    /// <summary>Gets a value indicating whether the instance has been fully inflated.</summary>
    internal bool Inflated { get; private set; }

    /// <summary>Gets the original health of the instance, before it was piped.</summary>
    internal int OriginalHealth { get; }

    /// <summary>Gets the original aggro range of the instance, before it was piped.</summary>
    internal int OriginalRange { get; }

    /// <summary>Gets the original scale of the instance, before it was piped.</summary>
    internal float OriginalScale { get; }

    /// <summary>Gets the fake <see cref="Farmer"/> instance used to aggro other <see cref="Monster"/>s.</summary>
    internal FakeFarmer FakeFarmer { get; }

    /// <summary>Grows the <see cref="PipedSlime"/> one stage.</summary>
    internal void Inflate()
    {
        this.Instance.Scale = Math.Min(this.Instance.Scale * 1.1f, Math.Min(this.OriginalScale * 2f, 2f));
        if (this.Instance.Scale <= 1.4f || (this.Instance.Scale < this.OriginalScale * 2f &&
                                    Game1.random.NextDouble() > 0.2 - (Game1.player.DailyLuck / 2) -
                                    (Game1.player.LuckLevel * 0.01)))
        {
            return;
        }

        this.Instance.MaxHealth += (int)Math.Round(this.Instance.Health * this.Instance.Scale * this.Instance.Scale);
        this.Instance.Health = this.Instance.MaxHealth;
        this.Instance.moveTowardPlayerThreshold.Value = 9999;
        if (Game1.random.NextDouble() < 1d / 3d)
        {
            this.Instance.addedSpeed += Game1.random.Next(3);
        }

        if (this.Instance.Scale >= 1.8f)
        {
            this.Instance.willDestroyObjectsUnderfoot = true;
        }

        this.Inflated = true;
    }

    /// <summary>Shrinks this <see cref="Piper"/> one stage.</summary>
    internal void Deflate()
    {
        this.Instance.Scale = Math.Max(this.Instance.Scale / 1.1f, this.OriginalScale);
        if (this.Instance.Scale > this.OriginalScale)
        {
            return;
        }

        this.Instance.MaxHealth = this.OriginalHealth;
        this.Instance.Health = this.Instance.MaxHealth;
        this.Instance.moveTowardPlayerThreshold.Value = this.OriginalRange;
        this.Instance.willDestroyObjectsUnderfoot = false;
        this.Instance.addedSpeed = 0;
        this.Instance.focusedOnFarmers = false;
    }
}
