namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class DuggyBehaviorAtGameTickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DuggyBehaviorAtGameTickPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal DuggyBehaviorAtGameTickPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Duggy>(nameof(Duggy.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher in ambush from Duggies.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? DuggyBehaviorAtGameTickTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (Sprite.currentFrame < 4)
        // To: if (Sprite.currentFrame < 4 && !player.IsInAmbush())
        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_I4_4)])
                .Move()
                .GetOperand(out var dontDoDamage)
                .Move()
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Monster).RequirePropertyGetter(nameof(Monster.Player))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.IsAmbushing))),
                        new CodeInstruction(OpCodes.Brtrue, dontDoDamage),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed hiding ambushing Poacher from Duggies.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
