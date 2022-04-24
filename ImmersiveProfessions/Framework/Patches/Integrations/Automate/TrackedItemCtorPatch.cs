using System;
using StardewValley.Tools;

#nullable enable
namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;

using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;

#endregion using directives

[UsedImplicitly]
internal class TrackedItemCtorPatch : BasePatch
{
    private static readonly FieldInfo _Item = "Pathoschild.Stardew.Automate.TrackedItem".ToType().RequireField("Item")!;

    /// <summary>Construct an instance.</summary>
    internal TrackedItemCtorPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.TrackedItem".ToType().GetConstructor(new[] {typeof(Item), typeof(Action<Item>), typeof(Action<Item>)});
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to fix collected rings from crab pots.</summary>
    [HarmonyPostfix]
    private static void TrackedItemCtorPostfix(object __instance, Item item)
    {
        if (!item.ParentSheetIndex.IsAnyOf(14, 51, 516, 517, 518, 519, 527, 529, 530, 531, 532,
                533, 534)) return;

        if (item.ParentSheetIndex.IsAnyOf(14, 51))
            _Item.SetValue(__instance, new MeleeWeapon(item.ParentSheetIndex));
        else
            _Item.SetValue(__instance, new Ring(item.ParentSheetIndex));
    }

    #endregion harmony patches
}