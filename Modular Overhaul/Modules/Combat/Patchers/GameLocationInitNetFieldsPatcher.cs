namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationInitNetFieldsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationInitNetFieldsPatcher"/> class.</summary>
    internal GameLocationInitNetFieldsPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>("initNetFields");
    }

    #region harmony patches

    [HarmonyPostfix]
    private static void GameLocationInitNetFieldsPostfix(GameLocation __instance)
    {
        __instance.characters.OnValueRemoved += OnCharacterRemoved;
    }

    #endregion harmony patches

    #region injected subroutines

    private static void OnCharacterRemoved(NPC npc)
    {
        if (npc is not Monster { Health: > 0 } monster)
        {
            return;
        }

        Log.D($"{monster.Name} was removed before it was dead. Re-adding...");
        monster.currentLocation.characters.Add(monster);
    }

    #endregion injected subroutines
}
