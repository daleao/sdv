namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformObjectDropInActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformObjectDropInActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectPerformObjectDropInActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performObjectDropInAction));
    }

    #region harmony patches

    /// <summary>Patch to remember initial machine state.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    [UsedImplicitly]
    private static bool ObjectPerformObjectDropInActionPrefix(SObject __instance, out bool __state, bool probe)
    {
        __state = __instance.heldObject.Value !=
                  null && !probe; // remember whether this machine was already holding an object
        return true; // run original logic
    }

    /// <summary>Patch to increase Artisan production + integrate Quality Artisan Products + Immersive Diary Yield tweak.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectPerformObjectDropInActionPostfix(
        SObject __instance, bool __state, Item dropInItem, Farmer who)
    {
        // if there was an object inside before running the original method, or if the machine is not an artisan machine, or if the machine is still empty after running the original method, then do nothing
        if (__state || !__instance.IsArtisanMachine() || __instance.heldObject.Value is not { } output ||
            dropInItem is not SObject input)
        {
            return;
        }

        var user = who;
        var owner = __instance.GetOwner();
        var r = new Random(Guid.NewGuid().GetHashCode());
        var newQuality = ObjectQuality.Regular;

        // artisan users can preserve the input quality
        if (__instance.QualifiedItemId != QIDs.Cask && user.HasProfession(Profession.Artisan))
        {
            newQuality = (ObjectQuality)input.Quality;
            if (!user.HasProfession(Profession.Artisan, true))
            {
                if (r.NextDouble() > who.FarmingLevel / 30d)
                {
                    newQuality = newQuality.Decrement();
                    if (r.NextDouble() > who.FarmingLevel / 15d)
                    {
                        newQuality = newQuality.Decrement();
                    }
                }
            }
        }

        output.Quality = Math.Max(output.Quality, (int)newQuality);

        // artisan-owned machines work faster and may upgrade quality
        if (!owner.HasProfessionOrLax(Profession.Artisan))
        {
            return;
        }

        if (output.Quality < SObject.bestQuality && __instance.QualifiedItemId != QIDs.Cask && r.NextBool(0.05))
        {
            output.Quality += output.Quality == SObject.highQuality ? 2 : 1;
        }

        __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 10;
    }

    #endregion harmony patches
}
