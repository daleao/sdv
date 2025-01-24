<div align="center">

# Wildcat - Dynamic Combat

</div>

## What This Is

This mod seeks to changes the feel of combat from a button mash-fest to a more strategic and dynamic experience allowing for some expression of player skill and buildcraft. It does this mainly by introducing a multi-hit combo system inspired by Haunted Chocolatier trailers, accompanied by tweaks to how statistics other than flat damage affect your overall performance in combat.

This is not a casual mod. It will make combat an overall more difficult and more frustrating, but hopefully more rewarding, experience.


## Combo Framework

Weapon spamming is a real problem in vanilla;
- It means that more offense is always the best defense, which renders both the defense stat and defensive special move of the sword completely useless.
- It lessens the impact of attack speed bonuses, which in vanilla only affects a single 1 out of the 4 ~ 5 frames in the attack swipe animation for swords and clubs.
- It turns knockback into more of a hindrance, since you can't spam click an enemy that gets pushed outside your attack range.

All of this contributes to the lack of variety in combat encounters and player builds, with flat damage becoming the only real viable option.

We propose a solution by implementing a combo framework for melee weapons. A combo is a short burst of continuous swings, followed by a short, forced "cooldown". Each weapon type has a configurable combo limit:
- Swords: up to 4 horizontal swipes, by default.
- Clubs: up to 2 hits, being one horizontal swipe and one downward smash, by default.
- Daggers: unchanged, effectively up to infinite hits.

You now actually have a reason to weave special moves in between regular attacks. Attack speed bonuses from Emerald rings and weapon forges will now serve to reduce both the cooldown between combos as well as the duration of every frame in the attack animation.


## Control Tweaks

### Slick Moves

To improve your mobility during combos, your attacks while running will preserve momentum, allowing you to drift in the direction of movement while attacking. This gives the impression of a faster-paced and more fluid combat.

### Face Mouse CUrsor

When playing with mouse and keyboard, enabling this feature will cause the farmer to swing their weapon in the direction of the mouse cursor. Combined with slick moves, this allows you to drift in a direction while attacking behind you. Mastering this mechanic will be essential to avoid hits during combat.


## Statistic Tweaks

### Defense

Even with the removal weapon spamming, defense still sucks. The vanilla game uses a simple linear vanilla formula for mitigating damage, which simply subtracts the defender's defense from the raw damage value:

`damage -= defense`

The problem is that damage continually scales as you encounter tougher enemies by mining deeper Mine levels, visiting different dungeons, or the hard mode of the Mines. Your defense, however, does not. This means that 1 point of defense can make a big different at the very start of the game, but quickly falls off and becomes completely useless. To make thing worse, the game also soft caps your defense if it reaches or surpasses 50% of the raw damage value (as if you could even reach that high) by subtracting a random amount (between 1/10 and 1/3) of your defense before applying the mitigation formula.

This mod replaces the vanilla mitigation formula with a very simple exponential formula:

`damage *= 0.9 ^ defense`

This results in a geometric progression, which causes every additional point of defense to mitigate exactly 10% more damage, guaranteeing that defense is always equally useful throughout every point in the game.

Moreover, the defense stat will also increase reflected damage, which includes the sword's parry special move as well as the Ring of Thorns.

Defensive builds focusing on Topaz rings and weapon forges should now actually be viable, even offensively.


### Knockback

In many games, when a character hits a wall or an object while being knocked back they will suffer environment damage. This mod introduces a similar mechanic. This creates an additional strategic layer to combat, where fighting enemies close to stones, walls or other objects can be more favorable than combat in open areas. The amount of environment damage suffered depends on the velocity of the character at the moment of collision, which of course is increased by the knockback stat from Amethyst rings and weapon forges.

Now, knocking enemies away from your combo is no longer necessarily a bad thing.


### Critical Strikes

So far, we've introduced tweaks to attack speed, defense and knockback. To round off the roster, we introduce two tweaks to critical strikes:

    Critical strikes ignore defense: This adds a layer of counter-play against tough defensive enemies, creating a rock-paper-scissors dynamic between various stats.
    Critical back strikes: Successful strikes to enemies from behind benefit from double critical strike chance. While this is ordinarily impossible against most enemies in vanilla, the DaLion suite of mods introduces various forms of Crowd Control that you may be able to abuse for this effect.


## Enemy Tweaks

Lastly, this mod provides optional sliders for tweaking enemy statistics including Health, Damage and Defense. There is also a toggle for randomizing encounters based on Daily Luck to create more varied dungeon experiences.


## Compatibility

N/A.


## Credits & Special Thanks

Credit to DjStln and NormanPCN for Combat Controls and Redux, respectively, from which the Slick Moves and Face Mouse Cursor features originate.
