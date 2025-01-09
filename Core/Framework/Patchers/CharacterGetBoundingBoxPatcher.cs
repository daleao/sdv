namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class CharacterGetBoundingBoxPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CharacterGetBoundingBoxPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CharacterGetBoundingBoxPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Character>(nameof(Character.GetBoundingBox));
    }

    #region harmony patches

    /// <summary>Extend Lava Lurk hitbox.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CharacterGetBoundingBoxPostfix(Character __instance, ref Rectangle __result)
    {
        if (__instance is not LavaLurk lurk)
        {
            return;
        }

        switch (lurk.currentState.Value)
        {
            case LavaLurk.State.Lurking:
                __result.Y -= __result.Height / 2;
                return;
            case LavaLurk.State.Emerged:
            case LavaLurk.State.Firing:
                __result.Y -= __result.Height;
                __result.Height += __result.Height / 2;
                return;
            default:
                __result.Height /= 2;
                return;
        }
    }

    #endregion harmony patches
}
