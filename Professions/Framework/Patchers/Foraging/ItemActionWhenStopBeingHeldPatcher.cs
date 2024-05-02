namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemActionWhenStopBeingHeldPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemActionWhenStopBeingHeldPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ItemActionWhenStopBeingHeldPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Item>(nameof(Item.actionWhenStopBeingHeld));
    }

    #region harmony patches

    /// <summary>Patch to reset Prestiged Ecologist edibility.</summary>
    [HarmonyPostfix]
    private static void ItemActionWhenStopBeingHeldPostfix(Item __instance, Farmer who)
    {
        if (__instance is SObject @object && @object.isForage() && who.HasProfession(Profession.Ecologist, true))
        {
            @object.Edibility = Game1.objectData[__instance.ItemId].Edibility;
        }
    }

    #endregion harmony patches
}
