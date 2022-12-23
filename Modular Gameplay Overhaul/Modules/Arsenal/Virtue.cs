namespace DaLion.Overhaul.Modules.Arsenal;

#region using directives

using System.Linq;
using Ardalis.SmartEnum;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

/// <summary>Represents one of the five heroic virtues.</summary>
public class Virtue : SmartEnum<Virtue>
{
    #region enum values

    /// <summary>
    ///     Honor cannot be purchased. Honor also cannot be sold, for its value is greater than all the treasure in the world.
    ///     Yet one can lose it, and whoever does so shall have sullied his name for all eternity. A truly honorable man always
    ///     stands behind his actions, faces every challenge and refuses to lie.
    /// </summary>
    public static readonly Virtue Honor = new("Honor", 144706);

    /// <summary>
    ///     There are many traits that bear witness to a man's true nature. Compassion is what separates men from beasts.
    ///     Whoever feels sympathy for his fellow man will never turn a blind eye to misfortune. He will always stand in
    ///     defense of the wronged.
    /// </summary>
    public static readonly Virtue Compassion = new("Compassion", 144707);

    /// <summary>
    ///     Wisdom is a virtue which one should strive to cultivate throughout one's life, for it is impossible to be so wise
    ///     one cannot become even wiser. The wise know this... As we journey through life, we should seek to make wise choices
    ///     Remember, wise choices are not those which make our lives easier or simpler. Often, they make them more complicated.
    ///     But always, they make us better.
    /// </summary>
    public static readonly Virtue Wisdom = new("Wisdom", 144708);

    /// <summary>
    ///     No man can be called good who does not share his prosperity with others. Generosity is required for dignity
    ///     in life and peace in death.
    /// </summary>
    public static readonly Virtue Generosity = new("Generosity", 144709);

    /// <summary>
    ///     Valor does not make one good, yet how many good men have you met in your life's journey who were cowards?
    ///     Those who posses valor do not hesitate to stand against the majority, no matter what the consequences.
    /// </summary>
    public static readonly Virtue Valor = new("Valor", 144710);

    #endregion enum values

    /// <summary>Initializes a new instance of the <see cref="Virtue"/> class.</summary>
    /// <param name="name">The name of the virtue.</param>
    /// <param name="value">The ID of the associated quest.</param>
    protected Virtue(string name, int value)
        : base(name, value)
    {
    }

    /// <summary>Checks if the <paramref name="farmer"/> has met the conditions for all virtues.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if all five virtue's conditions have been met, otherwise <see langword="false"/>.</returns>
    internal static bool AllProvenBy(Farmer farmer)
    {
        return List.All(virtue => virtue.ProvenBy(farmer));
    }

    /// <summary>Checks if the <paramref name="farmer"/> has met the condition for this virtue.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the virtue's condition has been met, otherwise <see langword="false"/>.</returns>
    internal bool ProvenBy(Farmer farmer)
    {
        var proven = false;
        this
            .When(Honor).Then(() => proven = farmer.Read<int>(DataFields.ProvenHonor) >= 3)
            .When(Compassion).Then(() => proven = farmer.Read<int>(DataFields.ProvenCompassion) >= 3)
            .When(Wisdom).Then(() => proven = farmer.Read<int>(DataFields.ProvenWisdom) >= 3)
            .When(Generosity).Then(() => proven = farmer.Read<bool>(DataFields.ProvenGenerosity))
            .When(Valor).Then(() => proven = farmer.Read<bool>(DataFields.ProvenValor));
        return proven;
    }

    /// <summary>Marks the corresponding quest as complete if the virtue has been proven.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    internal void CheckForCompletion(Farmer farmer)
    {
        if (!farmer.hasQuest(this) || !this.ProvenBy(farmer))
        {
            return;
        }

        farmer.completeQuest(this);
        Shared.Networking.Broadcaster.SendPublicChat($"{farmer.Name} has proven their {this}.");
        if (farmer.hasQuest(Constants.VirtuesNextQuestId) && AllProvenBy(farmer))
        {
            farmer.completeQuest(Constants.VirtuesNextQuestId);
        }
    }
}
