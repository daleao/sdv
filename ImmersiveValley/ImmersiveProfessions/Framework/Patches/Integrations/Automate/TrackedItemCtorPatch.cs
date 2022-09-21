namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using System;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
internal sealed class TrackedItemCtorPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="TrackedItemCtorPatch"/> class.</summary>
    internal TrackedItemCtorPatch()
    {
        this.Target = "Pathoschild.Stardew.Automate.TrackedItem".ToType()
            .RequireConstructor(typeof(Item), typeof(Action<Item>), typeof(Action<Item>));
    }

    #region harmony patches

    /// <summary>Patch to fix collected rings from crab pots.</summary>
    [HarmonyPrefix]
    private static void TrackedItemCtorPrefix(ref Item item)
    {
        if (!item.ParentSheetIndex.IsAnyOf(14, 51, 516, 517, 518, 519, 527, 529, 530, 531, 532, 533, 534))
        {
            return;
        }

        item = item.ParentSheetIndex switch
        {
            14 or 51 => new MeleeWeapon(item.ParentSheetIndex),
            _ => new Ring(item.ParentSheetIndex),
        };
    }

    #endregion harmony patches
}
