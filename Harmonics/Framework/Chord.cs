namespace DaLion.Harmonics.Framework;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Enchantments;
using StardewValley.Tools;

#endregion using directives

/// <inheritdoc cref="IChord"/>
public sealed class Chord : IChord
{
    private static readonly double[] Range = Enumerable.Range(0, 360).Select(i => i * Math.PI / 180d).ToArray();

    private static List<double> _linSpace =
        MathUtils.LinSpace(0d, 1d, (int)Config.ChordSoundDuration / 100).ToList();

    private readonly ICue[] _cues;
    private readonly string _id;

    private int[] _pitches;
    private int _fadeStepIndex;
    private int _position;
    private int _richness;
    private double _phase;
    private double _period = 360d;
    private LightSource? _lightSource;
    private HarmonicInterval[][] _intervalMatrix = null!;

    /// <summary>Initializes a new instance of the <see cref="Chord"/> class.Construct a Dyad instance.</summary>
    /// <param name="first">The first <see cref="Gemstone"/> in the Dyad.</param>
    /// <param name="second">The second <see cref="Gemstone"/> in the  Dyad.</param>
    internal Chord(Gemstone first, Gemstone second)
    {
        this.Notes = first.Collect(second).ToArray();
        this._cues = new ICue[2];
        for (var i = 0; i < 2; i++)
        {
            this._cues[i] = Game1.soundBank.GetCue("SinWave");
        }

        this._pitches = new int[2];
        this.Harmonize();

        this._id = this.GetHashCode().ToString();
    }

    /// <summary>Initializes a new instance of the <see cref="Chord"/> class.Construct a Triad instance.</summary>
    /// <param name="first">The first <see cref="Gemstone"/> in the Triad.</param>
    /// <param name="second">The second <see cref="Gemstone"/> in the Triad.</param>
    /// <param name="third">The third <see cref="Gemstone"/> in the Triad.</param>
    internal Chord(Gemstone first, Gemstone second, Gemstone third)
    {
        this.Notes = first.Collect(second, third).ToArray();
        this._cues = new ICue[3];
        for (var i = 0; i < 3; i++)
        {
            this._cues[i] = Game1.soundBank.GetCue("SinWave");
        }

        this._pitches = new int[3];
        this.Harmonize();

        this._id = this.GetHashCode().ToString();
    }

    /// <summary>Initializes a new instance of the <see cref="Chord"/> class.Construct a Tetrad instance.</summary>
    /// <param name="first">The first <see cref="Gemstone"/> in the Tetrad.</param>
    /// <param name="second">The second <see cref="Gemstone"/> in the Tetrad.</param>
    /// <param name="third">The third <see cref="Gemstone"/> in the Tetrad.</param>
    /// <param name="fourth">The fourth <see cref="Gemstone"/> in the Tetrad.</param>
    internal Chord(Gemstone first, Gemstone second, Gemstone third, Gemstone fourth)
    {
        this.Notes = first.Collect(second, third, fourth).ToArray();
        this._cues = new ICue[4];
        for (var i = 0; i < 4; i++)
        {
            this._cues[i] = Game1.soundBank.GetCue("SinWave");
        }

        this._pitches = new int[4];
        this.Harmonize();

        this._id = this.GetHashCode().ToString();
    }

    /// <inheritdoc />
    public Gemstone[] Notes { get; }

    /// <inheritdoc />
    public Gemstone? Root { get; private set; }

    /// <inheritdoc />
    public double Amplitude { get; private set; }

    /// <summary>Gets the ID of the corresponding <see cref="Buff"/> instance.</summary>
    internal string BuffId => "{UniqueId}/{this._id}";

    /// <summary>Gets the total resonance of each <see cref="Gemstone"/> due to interference with its neighbors.</summary>
    internal Dictionary<Gemstone, double> ResonanceByGemstone { get; } = new();

    /// <summary>Gets the current radius of the <see cref="_lightSource"/>.</summary>
    private float LightSourceRadius =>
        (float)(this.Amplitude + (this.Amplitude / 10d * Math.Sin(this.Root!.GlowFrequency * this._phase)));

    internal static void RecalculateLinSpace()
    {
        _linSpace = MathUtils.LinSpace(0d, 1d, (int)Config.ChordSoundDuration / 100).ToList();
    }

    /// <summary>Adds resonance stat bonuses to the farmer.</summary>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    internal void Resonate(Farmer who)
    {
        var effects = new BuffEffects { MagneticRadius = { this._richness * 32f } };
        this.ResonanceByGemstone.ForEach(pair => pair.Key.Resonate(effects, (float)pair.Value));
        who.applyBuff(new Buff(this.BuffId, effects: effects));
        who.Get_ResonatingChords().Add(this);
        if (this._lightSource is not null)
        {
            who.currentLocation.sharedLights[this._lightSource.Id] = this._lightSource;
        }

        if (Config.AudibleGemstones)
        {
            this.PlayCues();
        }

        if (who.CurrentTool is MeleeWeapon weapon)
        {
            this.ResonateAllForges(weapon);
        }
    }

    /// <summary>Removes resonating stat bonuses from the farmer.</summary>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    internal void Quench(Farmer who)
    {
        if (!who.buffs.AppliedBuffs.TryGetValue(this.BuffId, out var buff))
        {
            return;
        }

        who.buffs.Remove(buff.id);
        who.Get_ResonatingChords().Remove(this);
        if (this._lightSource is not null)
        {
            who.currentLocation.removeLightSource(this._lightSource.Id);
        }

        if (Config.AudibleGemstones)
        {
            this.StopCues();
        }

        if (who.CurrentTool is MeleeWeapon weapon)
        {
            this.QuenchAllForges(weapon);
        }
    }

    /// <summary>Attempts to resonate with a single <paramref name="forge"/> in the specified <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="forge">The forge as <see cref="BaseWeaponEnchantment"/>.</param>
    internal void ResonateSingleForge(MeleeWeapon weapon, BaseWeaponEnchantment forge)
    {
        if (this.Root?.EnchantmentType != forge.GetType())
        {
            return;
        }

        this.Root.Resonate(weapon, forge);
        weapon.Get_ResonatingChords().Add(this);
    }

    /// <summary>Attempts to resonate with all enchantments in the specified <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal void ResonateAllForges(MeleeWeapon weapon)
    {
        if (this.Root is null)
        {
            return;
        }

        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
        {
            if (enchantment.GetType() != this.Root.EnchantmentType)
            {
                continue;
            }

            this.Root.Resonate(weapon, enchantment);
            weapon.Get_ResonatingChords().Add(this);
        }
    }

    /// <summary>Attempts to resonate with a single <paramref name="forge"/> in the specified <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="forge">The forge as <see cref="BaseWeaponEnchantment"/>.</param>
    internal void QuenchSingleForge(MeleeWeapon weapon, BaseWeaponEnchantment forge)
    {
        if (this.Root?.EnchantmentType != forge.GetType())
        {
            return;
        }

        this.Root.Quench(weapon, forge);
        weapon.Get_ResonatingChords().Remove(this);
    }

    /// <summary>Adds resonance stat bonuses to the farmer.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal void QuenchAllForges(MeleeWeapon weapon)
    {
        if (this.Root is null)
        {
            return;
        }

        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
        {
            if (enchantment.GetType() != this.Root.EnchantmentType)
            {
                continue;
            }

            this.Root.Quench(weapon, enchantment);
            weapon.Get_ResonatingChords().Remove(this);
        }
    }

    /// <summary>Adds the total resonance stat bonuses to the <paramref name="buffer"/>.</summary>
    /// <param name="buffer">A <see cref="StatBuffer"/> for aggregating stat bonuses.</param>
    internal void Buffer(StatBuffer buffer)
    {
        this.ResonanceByGemstone.ForEach(pair => pair.Key.Buffer(buffer, (float)pair.Value));
        buffer.MagneticRadius += this._richness * 32f;
    }

    /// <summary>Adds resonance effects to the new <paramref name="location"/>.</summary>
    /// <param name="location">The new location.</param>
    internal void OnNewLocation(GameLocation location)
    {
        if (this._lightSource is null)
        {
            return;
        }

        location.sharedLights[this._lightSource.Id] = this._lightSource;
    }

    /// <summary>Removes resonance effects from the old <paramref name="location"/>.</summary>
    /// <param name="location">The left location.</param>
    internal void OnLeaveLocation(GameLocation location)
    {
        if (this._lightSource is null)
        {
            return;
        }

        location.removeLightSource(this._lightSource.Id);
    }

    /// <summary>Begins playback of the sine wave <see cref="ICue"/> for each note in the <see cref="Chord"/>.</summary>
    internal void PlayCues()
    {
        // copied from Game1.playSoundPitched in order to manipulate the instance volume
        try
        {
            for (var i = 0; i < this.Notes.Length; i++)
            {
                var cue = this._cues[i];
                if (cue.IsPlaying)
                {
                    continue;
                }

                var pitch = this._pitches[i];
                cue.SetVariable("Pitch", pitch); // for some reason cues cannot remember their pitch
                if (this._richness > 0)
                {
                    DelayedAction.functionAfterDelay(this._cues[i].Play, i * 100);
                }
                else
                {
                    cue.Play();
                }

                try
                {
                    if (!cue.IsPitchBeingControlledByRPC)
                    {
                        cue.Pitch = Utility.Lerp(-1f, 1f, pitch / 2400f);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
        catch (Exception ex)
        {
            Game1.debugOutput = Game1.parseText(ex.Message);
            Log.E(ex.ToString());
        }

        var fadeSteps = (int)Config.ChordSoundDuration / 100;
        foreach (var step in Enumerable.Range(0, fadeSteps))
        {
            DelayedAction.functionAfterDelay(
                this.FadeCues,
                (step * 100) + (this._richness > 0 ? this.Notes.Length * 100 : 0));
        }
    }

    /// <summary>Updates resonance effects.</summary>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    internal void Update(Farmer who)
    {
        if (this.Root is null)
        {
            return;
        }

        this._position = (int)((this._position + 1) % this._period);
        this._phase = Range[this._position];
        if (this._lightSource is not null)
        {
            this._lightSource.radius.Value = this.LightSourceRadius;
            var offset = Vector2.Zero;
            if (who.shouldShadowBeOffset)
            {
                offset += who.drawOffset;
            }

            this._lightSource.position.Value = new Vector2(who.Position.X + 32f, who.Position.Y + 32) + offset;
        }
    }

    /// <summary>Initializes the <see cref="_lightSource"/> if a resonant harmony exists in the <see cref="Chord"/>.</summary>
    internal void ResetLightSource()
    {
        if (this._lightSource is null)
        {
            return;
        }

        Game1.player.currentLocation.removeLightSource(this._lightSource.Id);
        this._lightSource = null;
    }

    /// <summary>Evaluate the <see cref="HarmonicInterval"/>s between <see cref="Notes"/> and the resulting harmonies.</summary>
    private void Harmonize()
    {
        Array.Sort(this.Notes);
        var groupedNotes = this.Notes.GroupBy(n => n).ToList();
        var distinctNotes = groupedNotes.Select(g => g.Key).ToArray();

        // initialize resonances
        foreach (var note in distinctNotes)
        {
            this.ResonanceByGemstone[note] = 0f;
        }

        // build interval matrix
        var size = this.Notes.Length;
        this._intervalMatrix = new HarmonicInterval[size][];
        for (var i = 0; i < size; i++)
        {
            this._intervalMatrix[i] = new HarmonicInterval[size];
            for (var j = 0; j < size; j++)
            {
                this._intervalMatrix[i][j] = new HarmonicInterval(this.Notes[j], this.Notes[(j + i) % size]);
            }
        }

        // unison chords can be ignored
        if (distinctNotes.Length == 1)
        {
            return;
        }

        // determine root note
        var fifths = this._intervalMatrix
            .GroupByIntervalNumber()[IntervalNumber.Fifth]
            .Distinct()
            .ToArray();
        if (fifths.Length is 1 or 2)
        {
            this.Root = fifths[0].First;
            if (fifths.Length > 1)
            {
                if (fifths[1].First.IntervalWith(this.Root) == IntervalNumber.Third)
                {
                    this.Root = fifths[1].First;
                }
            }

            // reposition root note
            var shifts = this.Notes.ShiftUntilStartsWith(this.Root);
            if (shifts > 0)
            {
                this._intervalMatrix.ForEach(intervals => intervals.ShiftLeft(shifts));
            }

            this._cues[0].Pitch = this.Root.NaturalPitch;
            var seenIntervals = new HashSet<HarmonicInterval>();
            for (var i = 1; i < size; i++)
            {
                var intervalPitch = this.Root.Harmonics[(int)this._intervalMatrix[i][0].Number];
                if (intervalPitch < this.Root.NaturalPitch)
                {
                    intervalPitch += 1200;
                }

                if (!seenIntervals.Add(this._intervalMatrix[i][0]))
                {
                    intervalPitch += intervalPitch <= 1200 ? 1200 : -1200;
                }

                this._pitches[i] = intervalPitch;
            }
        }

        // evaluate total resonance by note
        var countByNote = groupedNotes.ToDictionary(g => g.Key, g => g.Count());
        this._intervalMatrix.
            GroupByGemstone()
            .ForEach(group =>
            {
                //var numbers = group
                //    .Select(i => i.Number)
                //    .ToHashSet();
                group.ForEach(i =>
                {
                    var resonance = 0d;
                    switch (i.Number)
                    {
                        case IntervalNumber.Unison:
                        case IntervalNumber.Octave:
                            break;

                        // the perfect intervals
                        case IntervalNumber.Fifth:
                            resonance = 1d / 2d;
                            break;
                        case IntervalNumber.Fourth:
                            resonance = 1d / 3d;
                            break;

                        // ternary tryad
                        case IntervalNumber.Third when group.Key == this.Root:
                            resonance = 1d / 5d;
                            if (this.Notes.Contains(this.Root.Fifth))
                            {
                                this._richness++;
                            }

                            break;

                        // the consonant intervals
                        case IntervalNumber.Third:
                        case IntervalNumber.Sixth:
                            resonance = 1d / 6d;
                            break;

                        // ternary tetrad
                        case IntervalNumber.Seventh when group.Key == this.Root:
                            resonance = 1 / 8d;
                            if (this.Notes.ContainsAll(this.Root.Third, this.Root.Fifth))
                            {
                                this._richness++;
                            }

                            break;

                        // the dissonant intervals
                        case IntervalNumber.Seventh:
                        case IntervalNumber.Second:
                            resonance = -1d / 8d;
                            break;

                        default:
                            ThrowHelperExtensions.ThrowUnexpectedEnumValueException(i.Number);
                            break;
                    }

                    this.ResonanceByGemstone[group.Key] += resonance / countByNote[i.First];
                });
            });

        if (this.Root is null)
        {
            return;
        }

        if (this._richness > 0)
        {
            this.ResonanceByGemstone[this.Root] *= this._richness > 1 ? 2f : 1.5f;
        }

        this.Amplitude = this.ResonanceByGemstone[this.Root];
        this._period = 360d / this.Root.GlowFrequency;
        this._lightSource = new LightSource(
            this._id,
            (int)Config.ResonanceLightsourceTexture,
            Vector2.Zero,
            (float)this.Amplitude,
            Config.ColorfulResonances ? this.Root.GlowColor : Color.Black,
            playerID: Game1.player.UniqueMultiplayerID);
    }

    /// <summary>Ceases playback of the sine wave <see cref="ICue"/> for each note in the <see cref="Chord"/>.</summary>
    private void StopCues()
    {
        for (var i = 0; i < this.Notes.Length; i++)
        {
            var cue = this._cues[i];
            if (!cue.IsPlaying)
            {
                continue;
            }

            cue.Stop(AudioStopOptions.Immediate);
            cue.Volume = 1f;
        }

        this._fadeStepIndex = 0;
    }

    /// <summary>Fades out the sound cue volumes.</summary>
    private void FadeCues()
    {
        if (++this._fadeStepIndex >= _linSpace.Count)
        {
            this.StopCues();
            return;
        }

        for (var i = 0; i < this.Notes.Length; i++)
        {
            var cue = this._cues[i];
            if ((float)MathUtils.BoundedSCurve(_linSpace[this._fadeStepIndex], 3d) is var newVolume && newVolume < cue.Volume)
            {
                cue.Volume = newVolume;
            }

            if (cue.Volume is > 0f and <= 0.01f)
            {
                cue.Stop(AudioStopOptions.Immediate);
            }
        }
    }
}
