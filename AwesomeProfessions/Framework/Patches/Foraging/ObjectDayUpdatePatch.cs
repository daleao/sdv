namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectDayUpdatePatch()
    {
        Original = RequireMethod<SObject>(nameof(SObject.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to add quality to Ecologist Mushroom Boxes.</summary>
    [HarmonyPostfix]
    private static void ObjectDayUpdatePostfix(SObject __instance)
    {
        if (!__instance.IsMushroomBox() || __instance.heldObject.Value is null ||
            !Game1.MasterPlayer.HasProfession("Ecologist"))
            return;

        __instance.heldObject.Value.Quality = Game1.MasterPlayer.GetEcologistForageQuality();
    }

    #endregion harmony patches
}