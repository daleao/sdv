namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using Extensions;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Reflection;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class MushroomBoxMachineGetOutputPatch : HarmonyPatch
{
    private static Func<object, SObject>? _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal MushroomBoxMachineGetOutputPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine".ToType()
                .RequireMethod("GetOutput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for automated Mushroom Box quality.</summary>
    private static void MushroomBoxMachineGetOutputPrefix(object __instance)
    {
        try
        {
            if (!ModEntry.Config.AgeMushroomBoxes) return;

            _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine")
                .CompileUnboundDelegate<Func<object, SObject>>();
            var machine = _GetMachine(__instance);
            if (machine.heldObject.Value is not { } held) return;

            var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
            if (!owner.professions.Contains(Farmer.botanist))
                held.Quality = held.GetQualityFromAge();
            else if (ModEntry.ProfessionsAPI is not null)
                held.Quality = Math.Max(ModEntry.ProfessionsAPI.GetEcologistForageQuality(owner), held.Quality);
            else
                held.Quality = SObject.bestQuality;

            if (ModEntry.Config.MushroomBoxesRewardExp)
                Game1.player.gainExperience(Farmer.foragingSkill, 1);
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}