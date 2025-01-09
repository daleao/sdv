namespace DaLion.Enchantments.Framework.Animations;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>The animated shield that accompanies the <see cref="VampiricEnchantment"/>.</summary>
public class ShieldAnimation : TemporaryAnimatedSprite
{
    /// <summary>Initializes a new instance of the <see cref="ShieldAnimation"/> class.</summary>
    /// <param name="who">The <see cref="Farmer"/> instance with the shield.</param>
    public ShieldAnimation(Farmer who)
        : base(
            $"{Manifest.UniqueID}_Shield",
            new Rectangle(0, 0, 128, 128),
            200,
            4,
            int.MaxValue,
            Vector2.Zero,
            false,
            false,
            999999f,
            0f,
            Color.Maroon,
            1.5f,
            0f,
            0f,
            0f)
    {
        var r = 64 * this.scale;
        this.Position = new Vector2((who.FarmerSprite.SpriteWidth / 2 * 4f) - r, -who.FarmerSprite.SpriteHeight * 3.6f);
        this.positionFollowsAttachedCharacter = true;
        this.attachedCharacter = who;
        var alpha = (who.health - who.maxHealth) / (who.maxHealth * 0.2f);
        this.alpha = alpha;
    }

    internal static ShieldAnimation? Instance { get; set; }

    /// <inheritdoc />
    public override bool update(GameTime time)
    {
        var result = base.update(time);
        var who = (Farmer)this.attachedCharacter;
        var alpha = (who.health - who.maxHealth) / (who.maxHealth * 0.2f);
        this.alpha = alpha;
        if (alpha <= 0f)
        {
            Instance = null;
        }

        return result;
    }
}
