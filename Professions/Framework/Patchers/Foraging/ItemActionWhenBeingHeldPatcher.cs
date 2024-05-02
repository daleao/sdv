namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemActionWhenBeingHeldPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemActionWhenBeingHeldPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ItemActionWhenBeingHeldPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Item>(nameof(Item.actionWhenBeingHeld));
    }

    #region harmony patches

    /// <summary>Patch to set Prestiged Ecologist edibility.</summary>
    [HarmonyPostfix]
    private static void ItemActionWhenBeingHeldPostfix(Item __instance, Farmer who)
    {
        if (__instance is SObject @object && @object.isForage() && who.HasProfession(Profession.Ecologist, true))
        {
            @object.Edibility = (int)(@object.Edibility * 4f / 3f);
        }
    }

    #endregion harmony patches
}
