namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class DialogueChooseResponsePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DialogueChooseResponsePatcher"/> class.</summary>
    internal DialogueChooseResponsePatcher()
    {
        this.Target = this.RequireMethod<Dialogue>(nameof(Dialogue.chooseResponse));
    }

    #region harmony patches

    /// <summary>Record virtues after dialogue response. This works for questions triggered by the `$q` dialogue command.</summary>
    [HarmonyPostfix]
    private static void EventAnswerDialoguePostfix(Dialogue __instance, Response response)
    {
        var speakerName = __instance.speaker.Name;
        var player = Game1.player;
        switch (response.responseKey)
        {
        // HONOR //

            // Event ID: 7 (Maru 4 hearts) | Location: Hospital
            case "Event_Hospital_3" when speakerName == "Maru":

            // Event ID: 16 (Pierre 6 hearts) | Location: SeedShop
            case "Event_naga2" when speakerName == "Pierre":

            // Event ID: 36 (Penny 6 hearts) | Location: Trailer
            case "event_cook3" when speakerName == "Penny":

            // Event ID: 46 (Sam 4 hearts) | Location: SamHouse
            case "event_snack1" when speakerName == "Sam":

            // Event ID: 58 (Harvey 6 hearts) | Location: SeedShop
            case "event_aerobics1" when speakerName == "Harvey":

            // Event ID: 288847 (Alex 8 hearts) | Location: Beach
            case "event_box1" when speakerName == "Alex":

                player.Increment(DataKeys.ProvenHonor);
                WeaponsModule.State.Quest?.UpdateVirtueProgress(Virtue.Honor);

                return;

            // Event ID: 7 (Maru 4 hearts) | Location: Hospital
            case "Event_Hospital_1" or "Event_Hospital_2" when speakerName == "Maru":

            // Event ID: 36 (Penny 6 hearts) | Location: Trailer
            case "event_cook1" when speakerName == "Penny":

            // Event ID: 46 (Sam 4 hearts) | Location: SamHouse
            case "event_snack2" when speakerName == "Sam":

            // Event ID: 100 (Kent 3 hearts) | Location: SamHouse
            case "event_popcorn3" when speakerName == "Kent":

                player.Increment(DataKeys.ProvenHonor, -1);
                return;

        // COMPASSION //

            // Event ID: 13 (Haley 6 hearts) | Location: Beach
            case "Event_beach2" when speakerName == "Haley":

            // Event ID: 51 (Leah 4 hearts) | Location: LeahHouse
            case "event_parents1" when speakerName == "Leah":

            // Event ID: 100 (Kent 3 hearts) | Location: SamHouse
            case "event_popcorn2" when speakerName == "Kent":

            // Event ID: 288847 (Alex 8 hearts) | Location: Beach
            case "event_box3" when speakerName == "Alex":

            // Event ID: 502969 (Linus 0 hearts) | Location: Town
            case "quickResponse3" when speakerName == "Linus":

                player.Increment(DataKeys.ProvenCompassion);
                WeaponsModule.State.Quest?.UpdateVirtueProgress(Virtue.Compassion);

                return;

            // Event ID: 13 (Haley 6 hearts) | Location: Beach
            case "Event_beach1" when speakerName == "Haley":

            // Event ID: 288847 (Alex 8 hearts) | Location: Beach
            case "event_box4" when speakerName == "Alex":

                player.Increment(DataKeys.ProvenCompassion, -1);
                return;

        // WISDOM //

            // Event ID: 11 (Haley 2 hearts) | Location: HaleyHouse
            case "Event_clean1" when speakerName == "Haley":

            // Event ID: 21 (Alex 5 hearts) | Location: JoshHouse
            case "Event_books2" when speakerName == "Alex":

            // Event ID: 25 (Demetrius 3 hearts) | Location: ScienceHouse
            case "Event_tomato2" when speakerName == "Demetrius":

            // Event ID: 34 (Penny 2 hearts) | Location: Town
            case "event_mail2" when speakerName == "Penny":

            // Event ID: 50 (Leah 2 hearts) | Location: LeahHouse
            case "event_sculpt1" when speakerName == "Leah":

            // Event ID: 56 (Harvey 2 hearts) | Location: JoshHouse
            case "event_george1" when speakerName == "Harvey":

            // Event ID: 97 (Clint 3 hearts) | Location: Saloon
            case "event_advice2" when speakerName == "Clint":

                player.Increment(DataKeys.ProvenWisdom);
                WeaponsModule.State.Quest?.UpdateVirtueProgress(Virtue.Wisdom);

                return;

            // Event ID: 56 (Harvey 2 hearts) | Location: JoshHouse
            case "event_george2" when speakerName == "Harvey":

                Game1.player.Increment(DataKeys.ProvenWisdom, -1);
                return;
        }
    }

    #endregion harmony patches
}
