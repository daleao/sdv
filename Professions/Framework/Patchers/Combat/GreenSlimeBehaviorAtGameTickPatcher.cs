namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeBehaviorAtGameTickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeBehaviorAtGameTickPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GreenSlimeBehaviorAtGameTickPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to prevent Frost Jelly spontaneous enrage when Piped.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GreenSlimeBehaviorAtGameTickTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: Monster.set_WasKnockedBack(true);
        // After: trajectory *= knockBackModifier;
        try
        {
            helper
                .PatternMatch(
                    [new CodeInstruction(OpCodes.Ldstr, "Frost Jelly")],
                    ILHelper.SearchOption.Last)
                .Move(2)
                .GetOperand(out var branch)
                .Move()
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(GreenSlime_Piped).RequireMethod(nameof(GreenSlime_Piped.Get_Piped))),
                    new CodeInstruction(OpCodes.Brtrue_S, (Label)branch),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing Frost Jelly spontaneous enrage.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
