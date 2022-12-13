namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Infinity;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationGetGalaxySwordPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationGetGalaxySwordPatcher"/> class.</summary>
    internal GameLocationGetGalaxySwordPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>("getGalaxySword");
    }

    #region harmony patches

    /// <summary>Convert cursed -> blessed enchantment + galaxysoul -> infinity enchatnment.</summary>
    [HarmonyPrefix]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    private static bool GameLocationGetGalaxySwordPrefix()
    {
        if (!ArsenalModule.Config.InfinityPlusOne)
        {
            return true; // run original logic
        }

        var player = Game1.player;
        var obtained = player.Read(DataFields.GalaxyArsenalObtained).ParseList<int>();
        int? chosen = null;
        foreach (var item in player.Items.Where(i => i is MeleeWeapon or Slingshot))
        {
            var type = item is MeleeWeapon weapon ? weapon.type.Value : -1;
            var galaxy = galaxyFromWeaponType(type);
            if (obtained.Contains(galaxy))
            {
                continue;
            }

            chosen = galaxy;
            break;
        }

        chosen ??= new[]
        {
            Constants.GalaxySwordIndex, Constants.GalaxyHammerIndex, Constants.GalaxyDaggerIndex,
            Constants.GalaxySlingshotIndex,
        }.Except(obtained).First();

        player.Append(DataFields.GalaxyArsenalObtained, chosen.Value.ToString());
        Game1.flashAlpha = 1f;
        player.holdUpItemThenMessage(new MeleeWeapon(chosen.Value));
        player.reduceActiveItemByOne();
        for (var i = 0; i < obtained.Count; i++)
        {
            player.reduceActiveItemByOne();
        }

        player.Items.First(i => i.ParentSheetIndex == SObject.iridiumBar).Stack -=
            ArsenalModule.Config.IridiumBarsRequiredForGalaxyArsenal;

        if (!player.addItemToInventoryBool(new MeleeWeapon(chosen.Value)))
        {
            Game1.createItemDebris(new MeleeWeapon(chosen.Value), Game1.player.getStandingPosition(), 1);
        }

        //player.mailReceived.Add("galaxySword"); --> don't add mail to prevent galaxy weapons from appearing in stores
        player.jitterStrength = 0f;
        Game1.screenGlowHold = false;
        Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
            .globalChatInfoMessage("GalaxySword", Game1.player.Name);
        return false; // don't run original logic

        int galaxyFromWeaponType(int type)
        {
            return type switch
            {
                MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword => Constants.GalaxySwordIndex,
                MeleeWeapon.dagger => Constants.GalaxyDaggerIndex,
                MeleeWeapon.club => Constants.GalaxyHammerIndex,
                _ => Constants.GalaxySlingshotIndex,
            };
        }
    }

    #endregion harmony patches
}
