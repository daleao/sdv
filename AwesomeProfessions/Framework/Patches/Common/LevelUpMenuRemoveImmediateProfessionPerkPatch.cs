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

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;
using SuperMode;

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

        // unregister Super Mode
        if (ModEntry.State.Value.SuperMode?.Index != (SuperModeIndex) whichProfession) return;

        if (Game1.player.professions.Any(p => p is >= 26 and < 30))
        {
            var firstIndex = (SuperModeIndex) Game1.player.professions.First(p => p is >= 26 and < 30);
            ModData.Write(DataField.SuperModeIndex, firstIndex.ToString());
#pragma warning disable CS8509
            ModEntry.State.Value.SuperMode = firstIndex switch
#pragma warning restore CS8509
            {
                SuperModeIndex.Brute => new BruteFury(),
                SuperModeIndex.Poacher => new PoacherColdBlood(),
                SuperModeIndex.Piper => new PiperEubstance(),
                SuperModeIndex.Desperado => new DesperadoTemerity()
            };
        }
        else
        {
            ModData.Write(DataField.SuperModeIndex, null);
            ModEntry.State.Value.SuperMode = null;
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
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}