namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;

using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingLoadDisplayFieldsPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CombinedRingLoadDisplayFieldsPatch()
    {
        Target = RequireMethod<CombinedRing>("loadDisplayFields");
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Iridium description is always first, and gemstone descriptions are grouped together.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CombinedRingsLoadDisplayFieldsPrefix(CombinedRing __instance, ref bool __result)
    {
        if (!ModEntry.Config.ForgeableIridiumBand || __instance.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I)
            return true; // don't run original logic

        if (Game1.objectInformation is null || __instance.indexInTileSheet is null)
        {
            __result = false;
            return false; // don't run original logic
        }

        var data = Game1.objectInformation[__instance.indexInTileSheet.Value].Split('/');
        __instance.displayName = data[4];
        __instance.description = data[5];

        int addedKnockback = 0, addedPrecision = 0, addedCritChance = 0, addedCritPower = 0, addedSwingSpeed = 0, addedDamage = 0, addedDefense = 0;
        foreach (var ring in __instance.combinedRings)
            switch (ring.ParentSheetIndex)
            {
                case Constants.AMETHYSTR_RING_INDEX_I:
                    addedKnockback += 10;
                    break;
                case Constants.TOPAZ_RING_INDEX_I:
                    if (ModEntry.Config.RebalancedRings) addedDefense += 3;
                    else addedPrecision += 10;
                    break;
                case Constants.AQUAMARINE_RING_INDEX_I:
                    addedCritChance += 10;
                    break;
                case Constants.JADE_RING_INDEX_I:
                    addedCritPower += ModEntry.Config.RebalancedRings ? 30 : 10;
                    break;
                case Constants.EMERALD_RING_INDEX_I:
                    addedSwingSpeed += 10;
                    break;
                case Constants.RUBY_RING_INDEX_I:
                    addedDamage += 10;
                    break;
            }

        if (addedKnockback > 0)
        {
            data = Game1.objectInformation[Constants.AMETHYSTR_RING_INDEX_I].Split('/');
            var description = Regex.Replace(data[5], @"\d{2}", addedKnockback.ToString());
            __instance.description += "\n\n" + description;
        }

        if (addedPrecision > 0)
        {
            data = Game1.objectInformation[Constants.TOPAZ_RING_INDEX_I].Split('/');
            var description = Regex.Replace(data[5], @"\d{2}", addedPrecision.ToString());
            __instance.description += "\n\n" + description;
        }

        if (addedCritChance > 0)
        {
            data = Game1.objectInformation[Constants.AQUAMARINE_RING_INDEX_I].Split('/');
            var description = Regex.Replace(data[5], @"\d{2}", addedCritChance.ToString());
            __instance.description += "\n\n" + description;
        }

        if (addedCritPower > 0)
        {
            data = Game1.objectInformation[Constants.JADE_RING_INDEX_I].Split('/');
            var description = Regex.Replace(data[5], @"\d{2}", addedCritPower.ToString());
            __instance.description += "\n\n" + description;
        }

        if (addedSwingSpeed > 0)
        {
            data = Game1.objectInformation[Constants.EMERALD_RING_INDEX_I].Split('/');
            var description = Regex.Replace(data[5], @"\d{2}", addedSwingSpeed.ToString());
            __instance.description += "\n\n" + description;
        }

        if (addedDamage > 0)
        {
            data = Game1.objectInformation[Constants.RUBY_RING_INDEX_I].Split('/');
            var description = Regex.Replace(data[5], @"\d{2}", addedDamage.ToString());
            __instance.description += "\n\n" + description;
        }

        if (addedDefense > 0)
        {
            var description = ModEntry.i18n.Get("rings.topaz").ToString();
            description = Regex.Replace(description, @"\d{1}", addedDefense.ToString());
            __instance.description += "\n\n" + description;
        }

        __instance.description = __instance.description.Trim();
        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}