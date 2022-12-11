namespace DaLion.Ligo.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodOpenTreasureMenuEndFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodOpenTreasureMenuEndFunctionPatcher"/> class.</summary>
    internal FishingRodOpenTreasureMenuEndFunctionPatcher()
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.openTreasureMenuEndFunction));
    }

    #region harmony patches

    /// <summary>Prevent obtaining copies of Neptune's Glaive.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishingRodOpenTreasureMenuEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.specialItems))),
                        new CodeInstruction(OpCodes.Ldc_I4_S), new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetIntList).RequireMethod(nameof(NetIntList.Contains))),
                    })
                .Match(new[]
                    {
                        new CodeInstruction(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.random))),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Tool).RequireField("lastUser")),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 14),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetIntList).RequireMethod(nameof(NetIntList.Add))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Neptune Glaive to unique list.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
