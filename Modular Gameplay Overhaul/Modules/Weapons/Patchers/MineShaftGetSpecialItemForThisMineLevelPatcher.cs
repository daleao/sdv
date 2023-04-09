namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Overhaul.Modules.Weapons.Extensions;
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
        if (!WeaponsModule.Config.EnableRebalance)
        {
            return true; // run original logic
        }

        try
        {
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

            switch (r.Next(4))
            {
                case 0:
                    __result = ChooseRing(level);
                    return false; // don't run original logic
                case 1:
                    __result = ChooseBoots(level);
                    return false; // don't run original logic
                default:
                    __result = ChooseWeapon(level, r);
                    return false; // don't run original logic
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    #region injected subroutines

    private static Item ChooseBoots(int level)
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

    private static Item ChooseRing(int level)
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
                        possibles.Add(new MeleeWeapon(ItemIDs.SteelSmallsword));
                        break;
                    case 1:
                        possibles.Add(new MeleeWeapon(ItemIDs.SilverSaber));
                        break;
                    case 2:
                        possibles.Add(new MeleeWeapon(ItemIDs.CarvingKnife));
                        break;
                    case 3:
                        possibles.Add(new MeleeWeapon(ItemIDs.WoodClub));
                        break;
                }

                break;
            case < 40:
                switch (r.Next(4))
                {
                    case 0:
                        possibles.Add(new MeleeWeapon(ItemIDs.Cutlass));
                        break;
                    case 1:
                        possibles.Add(new MeleeWeapon(ItemIDs.IronEdge));
                        break;
                    case 2:
                        possibles.Add(new MeleeWeapon(ItemIDs.BurglarsShank));
                        break;
                    case 3:
                        possibles.Add(new MeleeWeapon(ItemIDs.WoodMallet));
                        break;
                }

                if (level > 30 && r.NextDouble() < 0.2)
                {
                    possibles.Add(new MeleeWeapon(ItemIDs.ShadowDagger));
                }

                break;
            case < 80:
                switch (r.Next(4))
                {
                    case 0:
                        possibles.Add(new MeleeWeapon(ItemIDs.Rapier));
                        break;
                    case 1:
                        possibles.Add(new MeleeWeapon(ItemIDs.Claymore));
                        break;
                    case 2:
                        possibles.Add(new MeleeWeapon(ItemIDs.WindSpire));
                        break;
                    case 3:
                        possibles.Add(new MeleeWeapon(ItemIDs.LeadRod));
                        break;
                }

                if (r.NextDouble() < 0.5)
                {
                    possibles.Add(new MeleeWeapon(ItemIDs.CrystalDagger));
                }

                if (r.NextDouble() < 0.2)
                {
                    possibles.Add(new MeleeWeapon(ItemIDs.YetiTooth));
                }

                break;
            case < 120:
                switch (r.Next(4))
                {
                    case 0:
                        possibles.Add(new MeleeWeapon(ItemIDs.SteelFalchion));
                        break;
                    case 1:
                        possibles.Add(new MeleeWeapon(ItemIDs.TemperedBroadsword));
                        break;
                    case 2:
                        possibles.Add(new MeleeWeapon(ItemIDs.IronDirk));
                        break;
                    case 3:
                        possibles.Add(new MeleeWeapon(ItemIDs.Kudgel));
                        break;
                }

                break;
            default:
                switch (r.Next(3))
                {
                    case 0:
                        possibles.Add(new MeleeWeapon(ItemIDs.TemplarsBlade));
                        break;
                    case 1:
                        possibles.Add(new MeleeWeapon(ItemIDs.WickedKris));
                        break;
                    case 2:
                        possibles.Add(new MeleeWeapon(ItemIDs.TheSlammer));
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
            new Ring(ItemIDs.IridiumBand), // iridium band
            new MeleeWeapon(ItemIDs.IridiumNeedle), // iridium needle
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
