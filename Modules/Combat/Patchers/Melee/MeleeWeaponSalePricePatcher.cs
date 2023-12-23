// ReSharper disable PossibleLossOfFraction
namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponSalePricePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponSalePricePatcher"/> class.</summary>
    internal MeleeWeaponSalePricePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.salePrice));
    }

    #region harmony patches

    /// <summary>Adjust weapon sell price by level.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponSalePricePrefix(MeleeWeapon __instance, ref int __result)
    {
        if (!CombatModule.Config.WeaponsSlingshots.EnableOverhaul)
        {
            return true; // run original logic
        }

        try
        {
            var tier = WeaponTier.GetFor(__instance);
            if (tier == WeaponTier.Untiered)
            {
                return true; // run original logic
            }

            __result = tier == WeaponTier.Masterwork
                ? __instance.Name.StartsWith("Dragon")
                    ? (int)(tier.Price * 1.5)
                    : __instance.Name.StartsWith("Elvish")
                        ? (int)(tier.Price * 0.75)
                        : tier.Price
                : tier.Price;

            // bonus points for enchantments
            for (var i = 0; i < __instance.enchantments.Count; i++)
            {
                var enchantment = __instance.enchantments[i];
                if (enchantment is GalaxySoulEnchantment)
                {
                    __result += 2500 * enchantment.GetLevel(); // half of Galaxy Soul value
                }
                else if (enchantment.IsForge())
                {
#pragma warning disable SA1513 // Closing brace should be followed by blank line
                    __result += enchantment switch
                    {
                        RubyEnchantment => 125,
                        AquamarineEnchantment => 90,
                        AmethystEnchantment => 50,
                        GarnetEnchantment => 150,
                        EmeraldEnchantment => 125,
                        JadeEnchantment => 100,
                        TopazEnchantment => 40,
                        _ => 0,
                    } * enchantment.GetLevel(); // half of gemstone value
#pragma warning restore SA1513 // Closing brace should be followed by blank line
                }
                else if (!enchantment.IsSecondaryEnchantment())
                {
                    __result += 1000; // half of Prismatic Shard value
                }
            }

            __result = (int)(__result * 2f * Game1.player.difficultyModifier); // x2 because this number will later be halved by the game
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
