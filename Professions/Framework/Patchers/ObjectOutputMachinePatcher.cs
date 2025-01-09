namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectOutputMachinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectOutputMachinePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ObjectOutputMachinePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.OutputMachine));
    }

    #region harmony patches

    /// <summary>Patch to increase production frequency of Producer Bee House + set Prestiged Gemologist Crystalaria quality + double Prestiged Demolitionist coal generation.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectOutputMachinePostfix(SObject __instance, Item inputItem)
    {
        var held = __instance.heldObject.Value;
        var owner = __instance.GetOwner();
        switch (__instance.QualifiedItemId)
        {
            case QualifiedBigCraftableIds.BeeHouse when Config.BeesAreAnimals:
            {
                if (!owner.HasProfessionOrLax(Profession.Producer))
                {
                    break;
                }

                if (owner.HasProfession(Profession.Producer, true))
                {
                    __instance.MinutesUntilReady /= 3;
                    break;
                }

                __instance.MinutesUntilReady /= 2;
                break;
            }

            case QualifiedBigCraftableIds.Crystalarium when held is not null:
            {
                if (owner.HasProfession(Profession.Gemologist, true))
                {
                    held.Quality = ((SObject)inputItem).Quality;
                }

                break;
            }

            default:
                if (__instance.GetMachineData().IsIncubator && Game1.player.HasProfession(Profession.Breeder))
                {
                    __instance.MinutesUntilReady /= 2;
                }

                if (held?.Category == SObject.baitCategory && owner.HasProfession(Profession.Luremaster, true))
                {
                    held.Stack *= 2;
                }

                break;
        }
    }

    #endregion harmony patches
}
