namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuRevalidateHealthPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuRevalidateHealthPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal LevelUpMenuRevalidateHealthPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.RevalidateHealth));
    }

    #region harmony patches

    /// <summary>
    ///     Patch revalidate player health after changes to the combat skill + revalidate fish pond capacity after changes
    ///     to the fishing skill.
    /// </summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool LevelUpMenuRevalidateHealthPrefix(Farmer farmer)
    {
        var expectedMaxHealth = 100;
        if (farmer.mailReceived.Contains("qiCave"))
        {
            expectedMaxHealth += 25;
        }

        for (var i = 1; i <= farmer.combatLevel.Value; i++)
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

        if (farmer.maxHealth == expectedMaxHealth)
        {
            return false; // don't run original logic
        }

        Log.W($"Fixing max health of {farmer.Name}.\nCurrent: {farmer.maxHealth}\nExpected: {expectedMaxHealth}");
        farmer.maxHealth = expectedMaxHealth;
        farmer.health = Math.Min(farmer.maxHealth, farmer.health);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
