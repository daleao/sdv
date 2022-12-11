namespace DaLion.Ligo.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Tools.Configs;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class TreePerformToolActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreePerformToolActionPatcher"/> class.</summary>
    internal TreePerformToolActionPatcher()
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.performToolAction));
    }

    #region harmony patches

    /// <summary>Prevent clearing tree saplings.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? TreePerformToolActionTranspiler(
        IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                    })
                .Move(2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Tools))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ToolsConfig).RequirePropertyGetter(nameof(ToolsConfig.Scythe))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ScytheConfig).RequirePropertyGetter(nameof(ScytheConfig.ClearTreeSaplings))),
                        new CodeInstruction(OpCodes.And),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding sturdy saplings.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
