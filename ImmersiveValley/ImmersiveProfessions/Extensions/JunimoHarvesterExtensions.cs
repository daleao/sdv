namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using Common.Extensions.Reflection;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using System;

#endregion using directives

/// <summary>Extensions for the <see cref="JunimoHarvester"/> class.</summary>
public static class JunimoHarvesterExtensions
{
    private static readonly Lazy<Func<JunimoHarvester, JunimoHut?>> _GetHome = new(() =>
        typeof(JunimoHarvester).RequirePropertyGetter("home")
            .CompileUnboundDelegate<Func<JunimoHarvester, JunimoHut?>>());

    /// <summary>The the <see cref="Farmer"/> who built the <see cref="JunimoHut"/> which houses the <see cref="JunimoHarvester"/>.</summary>
    public static Farmer GetOwner(this JunimoHarvester junimo)
    {
        var home = _GetHome.Value(junimo);
        if (home is null) return Game1.MasterPlayer;

        return Game1.getFarmerMaybeOffline(home.owner.Value) ?? Game1.MasterPlayer;
    }
}