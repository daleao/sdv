namespace DaLion.Ligo.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class DuggyBehaviorAtGameTickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DuggyBehaviorAtGameTickPatcher"/> class.</summary>
    internal DuggyBehaviorAtGameTickPatcher()
    {
        this.Target = this.RequireMethod<Duggy>(nameof(Duggy.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher from Duggies during Ultimate.</summary>
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
                .FindFirst(new CodeInstruction(OpCodes.Ldc_I4_4))
                .Advance()
                .GetOperand(out var dontDoDamage)
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Monster).RequirePropertyGetter(nameof(Monster.Player))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.IsInAmbush))),
                    new CodeInstruction(OpCodes.Brtrue, dontDoDamage));
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
