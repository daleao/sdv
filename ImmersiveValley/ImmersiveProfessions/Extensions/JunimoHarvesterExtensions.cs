namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using DaLion.Common.Extensions.Reflection;
using StardewValley.Buildings;
using StardewValley.Characters;

#endregion using directives

/// <summary>Extensions for the <see cref="JunimoHarvester"/> class.</summary>
public static class JunimoHarvesterExtensions
{
    private static readonly Lazy<Func<JunimoHarvester, JunimoHut?>> GetHome = new(() =>
        typeof(JunimoHarvester)
            .RequirePropertyGetter("home")
            .CompileUnboundDelegate<Func<JunimoHarvester, JunimoHut?>>());

    /// <summary>
    ///     Gets the <see cref="Farmer"/> who built the <see cref="JunimoHut"/> which houses the
    ///     <paramref name="junimo"/>.
    /// </summary>
    /// <param name="junimo">The <see cref="JunimoHarvester"/>.</param>
    /// <returns>The <see cref="Farmer"/> instance who constructed the hut where the <paramref name="junimo"/> lives, or the host of the game session if not found.</returns>
    public static Farmer GetOwner(this JunimoHarvester junimo)
    {
        var home = GetHome.Value(junimo);
        if (home is null)
        {
            return Game1.MasterPlayer;
        }

        return Game1.getFarmerMaybeOffline(home.owner.Value) ?? Game1.MasterPlayer;
    }
}
