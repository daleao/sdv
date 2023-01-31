namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Tools.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[IgnoreWithMod("bcmpinc.HarvestWithScythe")]
internal sealed class ObjectPerformToolActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformToolActionPatcher"/> class.</summary>
    internal ObjectPerformToolActionPatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performToolAction));
    }

    #region harmony patches

    /// <summary>Patch to allow harvesting forage with scythe.</summary>
    [HarmonyPrefix]
    private static bool ObjectPerformToolActionPrefix(SObject __instance, ref bool __result, Tool t, GameLocation location)
    {
        if (t.IsScythe() != true || !__instance.IsHarvestableForage())
        {
            return true; // run original logic
        }

        var who = t.getLastFarmerToUse();
        var tileLocation = __instance.TileLocation;
        if (tileLocation.X == 0f && tileLocation.Y == 0f)
        {
            foreach (var (key, value) in location.Objects.Pairs)
            {
                if (value != __instance)
                {
                    continue;
                }

                tileLocation = key;
                break;
            }
        }

        var random = new Random(((int)Game1.uniqueIDForThisGame / 2) + (int)Game1.stats.DaysPlayed +
                                   (int)tileLocation.X + ((int)tileLocation.Y * 777));
        if (who.professions.Contains(Farmer.botanist))
        {
            __instance.Quality = ProfessionsModule.IsEnabled ? SObject.bestQuality : who.GetEcologistForageQuality();
        }
        else if (random.NextDouble() < who.ForagingLevel / 30f)
        {
            __instance.Quality = SObject.highQuality;
        }
        else if (random.NextDouble() < who.ForagingLevel / 15f)
        {
            __instance.Quality = SObject.medQuality;
        }

        tileLocation *= 64f;
        who.gainExperience(Farmer.foragingSkill, 7);
        Game1.createItemDebris(__instance.getOne(), tileLocation, -1);
        Game1.stats.ItemsForaged++;
        if (who.professions.Contains(Farmer.gatherer) &&
            random.NextDouble() < (who.professions.Contains(100 + Farmer.gatherer) ? 0.4 : 0.2))
        {
            Game1.createItemDebris(__instance.getOne(), tileLocation, -1);
            who.gainExperience(Farmer.foragingSkill, 7);
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
