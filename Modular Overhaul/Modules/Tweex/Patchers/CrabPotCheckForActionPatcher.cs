namespace DaLion.Overhaul.Modules.Tweex.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotCheckForActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotCheckForActionPatcher"/> class.</summary>
    internal CrabPotCheckForActionPatcher()
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.checkForAction));
    }

    #region harmony patches

    /// <summary>Trash does not consume bait.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? PCrabPotCheckForActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var skipBaitConsumption = generator.DefineLabel();
            var proceedToBaitConsumption = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetFieldBase<SObject, NetRef<SObject>>).RequirePropertySetter("Value")),
                        new CodeInstruction(OpCodes.Ldarg_1),
                    })
                .Move()
                .AddLabels(skipBaitConsumption)
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) })
                .AddLabels(proceedToBaitConsumption)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Tweex))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.TrashDoesNotConsumeBait))),
                        new CodeInstruction(OpCodes.Brfalse_S, proceedToBaitConsumption),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Call, typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.IsTrash))),
                        new CodeInstruction(OpCodes.Brtrue_S, skipBaitConsumption),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed preventing trash bait consumption.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
