namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCtorPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotCtorPatch()
    {
        Target = RequireConstructor<Slingshot>(Type.EmptyTypes);
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        base.ApplyImpl(harmony);
        Target = RequireConstructor<Slingshot>(typeof(int));
        base.ApplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Add Rascal ammo slot.</summary>
    [HarmonyPostfix]
    private static void SlingshotCtorPostfix(Slingshot __instance)
    {
        if (!Game1.player.HasProfession(Profession.Rascal)) return;

        __instance.numAttachmentSlots.Value = 2;
        __instance.attachments.SetCount(2);
    }

    #endregion harmony patches
}