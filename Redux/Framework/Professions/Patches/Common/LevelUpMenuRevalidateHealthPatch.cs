namespace DaLion.Redux.Framework.Professions.Patches.Common;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Redux.Framework.Professions.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Menus;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuRevalidateHealthPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuRevalidateHealthPatch"/> class.</summary>
    internal LevelUpMenuRevalidateHealthPatch()
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.RevalidateHealth));
    }

    #region harmony patches

    /// <summary>
    ///     Patch revalidate player health after changes to the combat skill + revalidate fish pond capacity after changes
    ///     to the fishing skill.
    /// </summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuRevalidateHealthPrefix(Farmer farmer)
    {
        var expectedMaxHealth = 100;
        if (farmer.mailReceived.Contains("qiCave"))
        {
            expectedMaxHealth += 25;
        }

        for (var i = 1; i <= farmer.combatLevel.Value; ++i)
        {
            if (!farmer.newLevels.Contains(new Point(Skill.Combat, i)))
            {
                expectedMaxHealth += 5;
            }
        }

        if (farmer.HasProfession(Profession.Fighter))
        {
            expectedMaxHealth += 15;
        }

        if (farmer.HasProfession(Profession.Brute))
        {
            expectedMaxHealth += 25;
        }

        if (farmer.maxHealth != expectedMaxHealth)
        {
            Log.W($"Fixing max health of {farmer.Name}.\nCurrent: {farmer.maxHealth}\nExpected: {expectedMaxHealth}");
            farmer.maxHealth = expectedMaxHealth;
            farmer.health = Math.Min(farmer.maxHealth, farmer.health);
        }

        try
        {
            foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                         (p.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         !p.isUnderConstruction()))
            {
                // revalidate fish pond capacity
                pond.UpdateMaximumOccupancy();
                pond.currentOccupants.Value = Math.Min(pond.currentOccupants.Value, pond.maxOccupants.Value);
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return false; // don't run original logic
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
