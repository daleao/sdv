namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1CheckForEscapeKeysPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1CheckForEscapeKeysPatcher"/> class.</summary>
    internal Game1CheckForEscapeKeysPatcher()
    {
        this.Target = this.RequireMethod<Game1>("checkForEscapeKeysPatcher");
    }

    #region harmony patches

    /// <summary>Handle animation cancelling.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? Game1CheckForEscapeKeysTranspiler(
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
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertySetter(nameof(Farmer.UsingTool))),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Game1CheckForEscapeKeysPatcher).RequireMethod(nameof(HandleAnimationCancelled))),
                    }
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting animation cancel failsafe.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void HandleAnimationCancelled()
    {

    }

    #endregion injected subroutines
}
