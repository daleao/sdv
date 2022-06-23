namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Events;
using Textures;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticAssetsInvalidatedEvent : AssetsInvalidatedEvent
{
    /// <inheritdoc />
    protected override void OnAssetsInvalidatedImpl(object sender, AssetsInvalidatedEventArgs e)
    {
        if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/SkillBars")))
        {
            Textures.SkillBarTx =
                ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/SkillBars");
        }

        if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/UltimateMeter")))
        {
            Textures.UltimateMeterTx =
                ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/UltimateMeter");
        }

        if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"{ModEntry.Manifest.UniqueID}/PrestigeProgression")))
        {
            Textures.ProgressionTx =
                ModEntry.ModHelper.GameContent.Load<Texture2D>($"{ModEntry.Manifest.UniqueID}/PrestigeProgression");
        }
    }
}