using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class StaticGameLaunchedEvent : GameLaunchedEvent
	{
		/// <inheritdoc/>
		public override void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			// add Generic Mod Config Menu integration
			new Integrations.GenericModConfigMenuIntegrationForAwesomeTools(
				getConfig: () => ModEntry.Config,
				reset: () =>
				{
					ModEntry.Config = new ModConfig();
					ModEntry.ModHelper.WriteConfig(ModEntry.Config);
				},
				saveAndApply: () => { ModEntry.ModHelper.WriteConfig(ModEntry.Config); },
				log: ModEntry.Log,
				modRegistry: ModEntry.ModHelper.ModRegistry,
				manifest: ModEntry.Manifest
			).Register();
		}
	}
}