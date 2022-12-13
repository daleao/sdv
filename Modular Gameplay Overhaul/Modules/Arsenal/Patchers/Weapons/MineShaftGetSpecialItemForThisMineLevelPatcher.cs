namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftGetSpecialItemForThisMineLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftGetSpecialItemForThisMineLevelPatcher"/> class.</summary>
    internal MineShaftGetSpecialItemForThisMineLevelPatcher()
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.getSpecialItemForThisMineLevel));
    }

    #region harmony patches

    /// <summary>More consistent special item drops.</summary>
    [HarmonyPrefix]
    private static bool MineShaftGetSpecialItemForThisMineLevelPrefix(ref Item __result, int level)
    {
        if (!ArsenalModule.Config.Weapons.RebalancedStats)
        {
            return true; // run original logic
        }

        var r = new Random(Guid.NewGuid().GetHashCode());
        if (Game1.mine is null)
        {
            __result = new SObject(SObject.coal, 1);
            return false; // don't run original logic
        }

        if (Game1.mine.GetAdditionalDifficulty() > 0)
        {
            __result = ChooseHardMode(r);
            return false; // don't run original logic
        }

        switch (r.Next(3))
        {
            case 0:
                __result = ChooseBoots(level, r);
                return false; // don't run original logic
            case 1:
                __result = ChooseRing(level, r);
                return false; // don't run original logic
            case 2:
                __result = ChooseWeapon(level, r);
                return false; // don't run original logic
            default:
                __result = new SObject(SObject.coal, 1);
                return false; // don't run original logic
        }
    }

    #endregion harmony patches

    #region injected subroutines

    private static Item ChooseBoots(int level, Random r)
    {
        var possibles = new List<Item>();
        switch (level)
        {
            case < 40:
                possibles.Add(new Boots(505)); // rubber boots
                break;
            case < 80:
                possibles.Add(new Boots(510)); // thermal boots
                break;
            case < 120:
                possibles.Add(new Boots(511)); // dark boots
                break;
            default:
                possibles.Add(new Boots(513)); // space boots
                break;
        }

        return possibles.Choose();
    }

    private static Item ChooseRing(int level, Random r)
    {
        var possibles = new List<Item>();
        switch (level)
        {
            case < 80:
                possibles.Add(new Ring(516)); // small glow ring
                possibles.Add(new Ring(518)); // small magnet ring
                break;
            default:
                possibles.Add(new Ring(517)); // glow ring
                possibles.Add(new Ring(519)); // magnet ring
                if (level >= 100)
                {
                    possibles.Add(new Ring(887)); // immunity band
                }

                if (level >= 120)
                {
                    possibles.Add(new Ring(859)); // lucky ring
                    possibles.Add(new Ring(527)); // iridium band
                }

                break;
        }

        return possibles.Choose();
    }

    private static Item ChooseWeapon(int level, Random r)
    {
        var possibles = new List<Item>();
        switch (level)
        {
            case < 20:
                switch (r.Next(4))
                {
                    case 0:
                        possibles.Add(new MeleeWeapon(Constants.SteelSmallswordIndex));
                        break;
                    case 1:
                        possibles.Add(new MeleeWeapon(Constants.SilverSaberIndex));
                        break;
                    case 2:
                        possibles.Add(new MeleeWeapon(Constants.CarvingKnife));
                        break;
                    case 3:
                        possibles.Add(new MeleeWeapon(Constants.WoodClubIndex));
                        break;
                }

                break;
            case < 40:
                switch (r.Next(4))
                {
                    case 0:
                        possibles.Add(new MeleeWeapon(Constants.CutlassIndex));
                        break;
                    case 1:
                        possibles.Add(new MeleeWeapon(Constants.IronEdgeIndex));
                        break;
                    case 2:
                        possibles.Add(new MeleeWeapon(Constants.BurglarsShankIndex));
                        break;
                    case 3:
                        possibles.Add(new MeleeWeapon(Constants.WoodMalletIndex));
                        break;
                }

                if (level > 30 && r.NextDouble() < 0.2)
                {
                    possibles.Add(new MeleeWeapon(Constants.ShadowDaggerIndex));
                }

                break;
            case < 80:
                switch (r.Next(4))
                {
                    case 0:
                        possibles.Add(new MeleeWeapon(Constants.RapierIndex));
                        break;
                    case 1:
                        possibles.Add(new MeleeWeapon(Constants.ClaymoreIndex));
                        break;
                    case 2:
                        possibles.Add(new MeleeWeapon(Constants.WindSpireIndex));
                        break;
                    case 3:
                        possibles.Add(new MeleeWeapon(Constants.LeadRodIndex));
                        break;
                }

                if (r.NextDouble() < 0.5)
                {
                    possibles.Add(new MeleeWeapon(Constants.CrystalDaggerIndex));
                }

                if (r.NextDouble() < 0.2)
                {
                    possibles.Add(new MeleeWeapon(Constants.YetiToothIndex));
                }

                break;
            case < 120:
                switch (r.Next(4))
                {
                    case 0:
                        possibles.Add(new MeleeWeapon(Constants.SteelFalchionIndex));
                        break;
                    case 1:
                        possibles.Add(new MeleeWeapon(Constants.TemperedBroadswordIndex));
                        break;
                    case 2:
                        possibles.Add(new MeleeWeapon(Constants.IronDirkIndex));
                        break;
                    case 3:
                        possibles.Add(new MeleeWeapon(Constants.KudgelIndex));
                        break;
                }

                break;
            default:
                switch (r.Next(3))
                {
                    case 0:
                        possibles.Add(new MeleeWeapon(Constants.TemplarsBladeIndex));
                        break;
                    case 1:
                        possibles.Add(new MeleeWeapon(Constants.WickedKrisIndex));
                        break;
                    case 2:
                        possibles.Add(new MeleeWeapon(Constants.TheSlammerIndex));
                        break;
                }

                break;
        }

        return possibles.Choose(r);
    }

    private static Item ChooseHardMode(Random r)
    {
        return new List<Item>
        {
            new Boots(513), // space boots
            new Ring(Constants.IridiumBandIndex), // iridium band
            new MeleeWeapon(Constants.IridiumNeedleIndex), // iridium needle
            new SObject(909, r.Next(3, 9)), // radioactive bar
            new SObject(909, r.Next(2, 6)), // radioactive bar
            new SObject(909, r.Next(1, 3)), // radioactive bar
            new SObject(858, r.Next(1, 3)), // qi gem
            new SObject(858, 1), // qi gem
            new SObject(913, 1), // enricher
            new SObject(915, 1), // pressure nozzle
            MineShaft.getTreasureRoomItem(),
        }.Choose();
    }

    #endregion injected subroutines
}
