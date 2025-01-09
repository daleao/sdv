namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationUpdateWhenCurrentLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationUpdateWhenCurrentLocationPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationUpdateWhenCurrentLocationPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.UpdateWhenCurrentLocation));
    }

    #region harmony patches

    /// <summary>Stub for base <see cref="GameLocation.UpdateWhenCurrentLocation"/>.</summary>
    [HarmonyReversePatch]
    [MethodImpl(MethodImplOptions.NoInlining)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:Element parameters should be documented", Justification = "Reverse patch.")]
    internal static void GameLocationUpdateWhenCurrentLocationReverse(object instance, GameTime time)
    {
        // it's a stub so it has no initial content
        ThrowHelperExtensions.ThrowNotImplementedException("It's a stub.");
    }

    /// <summary>Patch to run Musk update.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GameLocationUpdateWhenCurrentLocationPostfix(GameLocation __instance, GameTime time)
    {
        if (time.TotalGameTime.Ticks % 60 == 0)
        {
            __instance.Get_Musks().ForEach(musk => musk.Update());
        }
    }

    #endregion harmony patches
}
