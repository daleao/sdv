//using JetBrains.Annotations;
//using StardewModdingAPI.Events;

//namespace TheLion.Stardew.Professions.Framework.Events
//{
//	[UsedImplicitly]
//	internal class StaticDayEndingEvent : DayEndingEvent
//	{
//		/// <inheritdoc />
//		public override void OnDayEnding(object sender, DayEndingEventArgs e)
//		{
//			// failsafe
//			ModEntry.Subscriber.CleanUpRogueEvents();
//			ModEntry.Data.CleanUpRogueDataFields();
//			ModEntry.Subscriber.SubscribeEventsForLocalPlayer();
//			ModEntry.Data.InitializeDataFieldsForLocalPlayer();
//		}
//	}
//}