namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class CharacterGetboundingBoxPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CharacterGetboundingBoxPatcher"/> class.</summary>
    internal CharacterGetboundingBoxPatcher()
    {
        this.Target = this.RequireMethod<Character>(nameof(Character.GetBoundingBox));
    }

    #region harmony patches

    /// <summary>Extend Lava Lurk hitbox.</summary>
    [HarmonyPostfix]
    private static void CharacterGetboundingBoxPostfix(Character __instance, ref Rectangle __result)
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
