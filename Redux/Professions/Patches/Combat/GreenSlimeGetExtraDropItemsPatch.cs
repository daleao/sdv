namespace DaLion.Redux.Professions.Patches.Combat;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Redux.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Deprecated]
internal sealed class GreenSlimeGetExtraDropItemsPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeGetExtraDropItemsPatch"/> class.</summary>
    internal GreenSlimeGetExtraDropItemsPatch()
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.getExtraDropItems));
    }

    #region harmony patches

    /// <summary>Patch Slime drop table for Piper.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeGetExtraDropItemsPostfix(GreenSlime __instance, List<Item> __result)
    {
        if (!__instance.currentLocation.DoesAnyPlayerHereHaveProfession(Profession.Piper, out var pipers) ||
            !Game1.MasterPlayer.mailReceived.Contains("slimeHutchBuilt"))
        {
            return;
        }

        var slimeCount =
            Game1.getFarm().buildings.Where(b =>
                    (b.owner.Value.IsIn(pipers.Select(p => p.UniqueMultiplayerID)) ||
                     !Context.IsMultiplayer) && b.indoors.Value is SlimeHutch && !b.isUnderConstruction() &&
                    b.indoors.Value.characters.Count > 0)
                .Sum(b => b.indoors.Value.characters.Count(npc => npc is GreenSlime)) +
            Game1.getFarm().characters.Count(npc => npc is GreenSlime);
        if (slimeCount == 0)
        {
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());
        var baseChance = (-1 / (0.02 * (slimeCount + 50))) + 1;

        // base drops
        var count = 0;
        while (r.NextDouble() < baseChance && count < 10)
        {
            __result.Add(new SObject(766, 1)); // slime
            if (r.NextDouble() < 5f / 8f)
            {
                __result.Add(new SObject(92, 1)); // sap
            }

            ++count;
        }

        if (MineShaft.lowestLevelReached >= 120 && __instance.currentLocation is MineShaft or VolcanoDungeon)
        {
            if (r.NextDouble() < baseChance / 8)
            {
                __result.Add(new SObject(72, 1)); // diamond
            }

            if (r.NextDouble() < baseChance / 10)
            {
                __result.Add(new SObject(74, 1)); // prismatic shard
            }
        }

        // color drops
        var color = __instance.color;
        if (__instance.Name != "Tiger Slime")
        {
            if (color.R < 80 && color.G < 80 && color.B < 80) // black
            {
                while (r.NextDouble() < baseChance / 2)
                {
                    __result.Add(new SObject(382, count)); // coal
                }

                if (r.NextDouble() < baseChance / 3)
                {
                    __result.Add(new SObject(553, 1)); // neptunite
                }

                if (r.NextDouble() < baseChance / 3)
                {
                    __result.Add(new SObject(539, 1)); // bixite
                }
            }
            else if (color.R > 200 && color.G > 180 && color.B < 50) // yellow
            {
                while (r.NextDouble() < baseChance / 2)
                {
                    __result.Add(new SObject(384, 1)); // gold ore
                }

                if (r.NextDouble() < baseChance / 3)
                {
                    __result.Add(new SObject(336, 1)); // gold bar
                }
            }
            else if (color.R > 220 && color.G is > 90 and < 150 && color.B < 50) // red
            {
                while (r.NextDouble() < baseChance / 2)
                {
                    __result.Add(new SObject(378, 1)); // copper ore
                }

                if (r.NextDouble() < baseChance / 3)
                {
                    __result.Add(new SObject(334, 1)); // copper bar
                }
            }
            else if (color.R > 150 && color.G > 150 && color.B > 150)
            {
                if (color.R > 230 && color.G > 230 && color.B > 230) // white
                {
                    while (r.NextDouble() < baseChance / 2)
                    {
                        __result.Add(new SObject(338, 1)); // refined quartz
                        __result.Add(new SObject(72, 1)); // diamond
                    }
                }
                else // grey
                {
                    while (r.NextDouble() < baseChance / 2)
                    {
                        __result.Add(new SObject(380, 1)); // iron ore
                    }

                    if (r.NextDouble() < baseChance / 3)
                    {
                        __result.Add(new SObject(335, 1)); // iron bar
                    }
                }
            }
            else if (color.R > 150 && color.B > 180 && color.G < 50) // purple
            {
                while (r.NextDouble() < baseChance / 3)
                {
                    __result.Add(new SObject(386, 1)); // iridium ore
                }

                if (r.NextDouble() < baseChance / 4)
                {
                    __result.Add(new SObject(337, 1)); // iridium bar
                }
            }

            if (!(r.NextDouble() < baseChance / 5))
            {
                return;
            }

            // slime eggs
            switch (__instance.Name)
            {
                case "Green Slime":
                    __result.Add(new SObject(680, 1));
                    break;

                case "Frost Jelly":
                    __result.Add(new SObject(413, 1));
                    break;

                case "Sludge":
                    __result.Add(color.B < 200 ? new SObject(437, 1) : new SObject(439, 1));
                    break;
            }
        }
        else
        {
            while (r.NextDouble() < baseChance)
            {
                switch (r.Next(4))
                {
                    case 0:
                        __result.Add(new SObject(831, 1)); // taro tuber
                        break;

                    case 1:
                        __result.Add(new SObject(829, 1)); // ginger
                        break;

                    case 2:
                        __result.Add(new SObject(833, 1)); // pineapple seeds
                        break;

                    case 3:
                        __result.Add(new SObject(835, 1)); // mango sapling
                        break;
                }
            }

            // tiger slime egg
            if (r.NextDouble() < baseChance / 5)
            {
                __result.Add(new SObject(857, 1));
            }
        }
    }

    #endregion harmony patches
}
