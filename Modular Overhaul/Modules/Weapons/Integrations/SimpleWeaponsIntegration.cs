namespace DaLion.Overhaul.Modules.Weapons.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

[RequiresMod("dengdeng.simpleweapons", "Simple Weapons")]
[IgnoreWithMod("Taiyo.VanillaTweaks")]
internal sealed class SimpleWeaponsIntegration : ModIntegration<SimpleWeaponsIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="SimpleWeaponsIntegration"/> class.</summary>
    internal SimpleWeaponsIntegration()
        : base("dengdeng.simpleweapons", "Simple Weapons", null, ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        ModHelper.GameContent.InvalidateCache("TileSheets/weapons");
        return true;
    }
}
