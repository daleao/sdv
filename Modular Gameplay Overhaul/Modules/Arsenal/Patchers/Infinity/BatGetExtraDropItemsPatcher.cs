namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Infinity;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BatGetExtraDropItemsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BatGetExtraDropItemsPatcher"/> class.</summary>
    internal BatGetExtraDropItemsPatcher()
    {
        this.Target = this.RequireMethod<Bat>(nameof(Bat.getExtraDropItems));
    }

    #region harmony patches

    /// <summary>Remove Dark Sword drop from Bat.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BatGetExtraDropItemsTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Removed: case 1
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_2),
                        new CodeInstruction(OpCodes.Newobj, typeof(MeleeWeapon).RequireConstructor(typeof(int))),
                    })
                .StripLabels(out var labels)
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<Item>).RequireMethod(nameof(List<Item>.Add))),
                    },
                    out var count)
                .Remove(count)
                .AddLabels(labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing Dark Sword drop from Bat.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
