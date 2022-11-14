namespace DaLion.Ligo.Modules.Professions.Extensions;

#region using directives

using StardewValley.Buildings;
using StardewValley.Characters;

#endregion using directives

/// <summary>Extensions for the <see cref="JunimoHarvester"/> class.</summary>
internal static class JunimoHarvesterExtensions
{
    /// <summary>
    ///     Gets the <see cref="Farmer"/> who built the <see cref="JunimoHut"/> which houses the
    ///     <paramref name="junimo"/>.
    /// </summary>
    /// <param name="junimo">The <see cref="JunimoHarvester"/>.</param>
    /// <returns>The <see cref="Farmer"/> instance who constructed the hut where the <paramref name="junimo"/> lives, or the host of the game session if not found.</returns>
    internal static Farmer GetOwner(this JunimoHarvester junimo)
    {
        var home = ModEntry.Reflector.GetUnboundPropertyGetter<JunimoHarvester, JunimoHut?>(junimo, "home")
            .Invoke(junimo);
        if (home is null)
        {
            return Game1.MasterPlayer;
        }

        return Game1.getFarmerMaybeOffline(home.owner.Value) ?? Game1.MasterPlayer;
    }
}
