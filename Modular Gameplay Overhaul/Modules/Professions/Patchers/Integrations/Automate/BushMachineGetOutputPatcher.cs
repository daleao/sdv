namespace DaLion.Overhaul.Modules.Professions.Patchers.Integrations;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
internal sealed class BushMachineGetOutputPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BushMachineGetOutputPatcher"/> class.</summary>
    internal BushMachineGetOutputPatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine"
            .ToType()
            .RequireMethod("GetOutput");
    }

    #region harmony patches

    /// <summary>Patch for automated Berry Bush quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BushMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: int quality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : 0);
        // To: int quality = GetOutputSubroutine(this);
        try
        {
            helper
                .FindProfessionCheck(Profession.Ecologist.Value) // find index of ecologist check
                .Move(-1)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            "Pathoschild.Stardew.Automate.Framework.BaseMachine`1".ToType()
                                .MakeGenericType(typeof(SObject))
                                .RequirePropertyGetter("Machine")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(BushMachineGetOutputPatcher).RequireMethod(nameof(GetOutputSubroutine))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_4) }, out var count)
                .Remove(count)
                .StripLabels();
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching automated Berry Bush quality." +
                  "\n—-- Do NOT report this to Automate's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int GetOutputSubroutine(Bush machine)
    {
        var chest = AutomateIntegration.Instance?.GetClosestContainerTo(machine);
        var user = ProfessionsModule.Config.LaxOwnershipRequirements ? Game1.player : chest?.GetOwner() ?? Game1.MasterPlayer;
        return user.HasProfession(Profession.Ecologist) ? user.GetEcologistForageQuality() : SObject.lowQuality;
    }

    #endregion injected subroutines
}
