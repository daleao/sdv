namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

using DaLion.Common.Harmony;
using Extensions;
using Ultimate;

#endregion using directives

[UsedImplicitly]
internal class LevelUpMenuRemoveImmediateProfessionPerkPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuRemoveImmediateProfessionPerkPatch()
    {
        Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.removeImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to remove modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuRemoveImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (!Enum.IsDefined(typeof(Profession), whichProfession)) return;

        var profession = (Profession) whichProfession;

        // remove immediate perks
        if (profession == Profession.Aquarist)
            foreach (var pond in Game1.getFarm().buildings.Where(p =>
                         (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         !p.isUnderConstruction() && p.maxOccupants.Value > 10))
            {
                pond.maxOccupants.Set(10);
                pond.currentOccupants.Value = Math.Min(pond.currentOccupants.Value, pond.maxOccupants.Value);
            }

        // unsubscribe unnecessary events
        EventManager.DisableAllForProfession(profession);

        // unregister Ultimate
        if (ModEntry.PlayerState.RegisteredUltimate?.Index != (UltimateIndex) whichProfession) return;

        if (Game1.player.professions.Any(p => p is >= 26 and < 30))
        {
            var firstIndex = (UltimateIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
            Game1.player.WriteData(DataField.UltimateIndex, firstIndex.ToString());
#pragma warning disable CS8509
            ModEntry.PlayerState.RegisteredUltimate = firstIndex switch
#pragma warning restore CS8509
            {
                UltimateIndex.BruteFrenzy => new Frenzy(),
                UltimateIndex.PoacherAmbush => new Ambush(),
                UltimateIndex.PiperPandemonium => new Pandemonium(),
                UltimateIndex.DesperadoBlossom => new DeathBlossom()
            };
        }
        else
        {
            Game1.player.WriteData(DataField.UltimateIndex, null);
            ModEntry.PlayerState.RegisteredUltimate = null;
        }
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> LevelUpMenuRemoveImmediateProfessionPerkTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: case <defender_id>:
        /// To: case <brute_id>:

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender)
                )
                .SetOperand((int) Profession.Brute);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}