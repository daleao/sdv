namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley;
using StardewValley.Buildings;

using Common;
using Common.Harmony;
using Common.Extensions.Reflection;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDoActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondDoActionPatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.doAction));
    }

    #region harmony patches

    /// <summary>Inject ItemGrabMenu + allow legendary fish to share a pond with their extended families.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FishPondDoActionTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (output.Value != null) {...} return true;
        /// To: if (output.Value != null)
        /// {
        ///     this.RewardExp(who);
        ///     return this.OpenChumBucketMenu();
        /// }

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.output))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetRef<Item>).RequirePropertyGetter(nameof(NetRef<Item>.Value))),
                    new CodeInstruction(OpCodes.Stloc_1)
                )
                .Retreat()
                .SetOpCode(OpCodes.Brfalse_S)
                .Advance()
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Ret)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishPondExtensions).RequireMethod(nameof(FishPondExtensions.RewardExp))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishPondExtensions).RequireMethod(nameof(FishPondExtensions.OpenChumBucketMenu)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding chum bucket menu.\nHelper returned {ex}");
            return null;
        }

        /// From: if (who.ActiveObject.ParentSheetIndex != (int) fishType)
        /// To: if (who.ActiveObject.ParentSheetIndex != (int) fishType && !IsExtendedFamily(who.ActiveObject.ParentSheetIndex, (int) fishType)

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.fishType))),
                    new CodeInstruction(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).RequireMethod("op_Implicit")),
                    new CodeInstruction(OpCodes.Beq)
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_0)
                )
                .GetInstructionsUntil(out var got, true, true,
                    new CodeInstruction(OpCodes.Beq)
                )
                .Insert(got)
                .Retreat()
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(Framework.Utils).RequireMethod(nameof(Framework.Utils.IsExtendedFamilyMember)))
                )
                .SetOpCode(OpCodes.Brtrue_S);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding family ties to legendary fish in ponds.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}