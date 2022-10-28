namespace DaLion.Redux.Rings.Resonance;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Redux.Rings.Events;
using DaLion.Redux.Rings.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <inheritdoc cref="IChord"/>
public sealed class Chord : IChord
{
    private static readonly double[] Range = Enumerable.Range(0, 120).Select(i => i / 120d).ToArray();
    private static int _position;

    private int _magnetism;
    private double _phase;
    private LightSource? _lightSource;

    /// <summary>Initializes a new instance of the <see cref="Chord"/> class.Construct a Dyad instance.</summary>
    /// <param name="first">The first <see cref="Gemstone"/> in the Dyad.</param>
    /// <param name="second">The second <see cref="Gemstone"/> in the  Dyad.</param>
    internal Chord(Gemstone first, Gemstone second)
    {
        this.Notes = first.Collect(second).ToArray();
        this.Harmonize();
        this.InitializeLightSource();
    }

    /// <summary>Initializes a new instance of the <see cref="Chord"/> class.Construct a Triad instance.</summary>
    /// <param name="first">The first <see cref="Gemstone"/> in the Triad.</param>
    /// <param name="second">The second <see cref="Gemstone"/> in the Triad.</param>
    /// <param name="third">The third <see cref="Gemstone"/> in the Triad.</param>
    internal Chord(Gemstone first, Gemstone second, Gemstone third)
    {
        this.Notes = first.Collect(second, third).ToArray();
        this.Harmonize();
        this.InitializeLightSource();
    }

    /// <summary>Initializes a new instance of the <see cref="Chord"/> class.Construct a Tetrad instance.</summary>
    /// <param name="first">The first <see cref="Gemstone"/> in the Tetrad.</param>
    /// <param name="second">The second <see cref="Gemstone"/> in the Tetrad.</param>
    /// <param name="third">The third <see cref="Gemstone"/> in the Tetrad.</param>
    /// <param name="fourth">The fourth <see cref="Gemstone"/> in the Tetrad.</param>
    internal Chord(Gemstone first, Gemstone second, Gemstone third, Gemstone fourth)
    {
        this.Notes = first.Collect(second, third, fourth).ToArray();
        this.Harmonize();
        this.InitializeLightSource();
    }

    /// <inheritdoc />
    public double Amplitude { get; private set; }

    /// <inheritdoc />
    public Gemstone? Root { get; private set; }

    /// <summary>Gets the <see cref="Gemstone"/>s that make up the <see cref="Chord"/>.</summary>
    /// <remarks>
    ///     The notes are sorted by resulting harmony, with the <see cref="Root"/> at index zero and remaining notes
    ///     ordered by increasing intervals with the former.
    /// </remarks>
    internal Gemstone[] Notes { get; }

    /// <summary>Gets the total resonance of each <see cref="Gemstone"/> due to interference with its neighbors.</summary>
    internal Dictionary<Gemstone, double> ResonanceByGemstone { get; } = new();

    /// <summary>Gets the <see cref="HarmonicInterval"/>s formed between each <see cref="Gemstone"/>.</summary>
    internal IGrouping<Gemstone, HarmonicInterval>[] GroupedIntervals { get; private set; } = null!; // set in harmonize


    /// <summary>Adds resonance stat bonuses to the farmer.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    public void Apply(GameLocation location, Farmer who)
    {
        ModEntry.State.Rings.ResonatingChords.Add(this);

        this.ResonanceByGemstone.ForEach(pair => pair.Key.ResonateInRing(who, (float)pair.Value));
        var distinctNotes = this.Notes.Distinct().ToArray();
        if (this.Notes.Length == 4 && distinctNotes.Length == 2 && distinctNotes.All(this.Notes.Except(distinctNotes).Contains))
        {
            this.ResonanceByGemstone.ForEach(pair => pair.Key.ResonateInRing(who, (float)pair.Value));
        }

        who.MagneticRadius += this._magnetism;
        if (this._lightSource is null)
        {
            return;
        }

        while (location.sharedLights.ContainsKey(this._lightSource.Identifier))
        {
            ++this._lightSource.Identifier;
        }

        location.sharedLights[this._lightSource.Identifier] = this._lightSource;
        ModEntry.Events.Enable<ResonanceUpdateTickedEvent>();

        if (this.Root is null)
        {
            return;
        }

        var tool = who.CurrentTool;
        if (tool is not (MeleeWeapon or Slingshot) || tool.enchantments.Count == 0 || !tool.HasEnchantmentOfType(this.Root))
        {
            return;
        }

        var count = tool.CountEnchantmentsOfType(this.Root);
        if (tool is MeleeWeapon weapon)
        {
            this.Root.ResonateInWeapon(weapon, (float)(this.Amplitude * count));
        }
        else if (tool is Slingshot slingshot)
        {
            this.Root.ResonateInSlingshot(slingshot, (float)(this.Amplitude * count));
        }
    }

    /// <summary>Removes resonating stat bonuses from the farmer.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    public void Unapply(GameLocation location, Farmer who)
    {
        ModEntry.State.Rings.ResonatingChords.Remove(this);

        this.ResonanceByGemstone.ForEach(pair => pair.Key.DissonateInRing(who, (float)pair.Value));
        var distinctNotes = this.Notes.Distinct().ToArray();
        if (this.Notes.Length == 4 && distinctNotes.Length == 2 && distinctNotes.All(this.Notes.Except(distinctNotes).Contains))
        {
            this.ResonanceByGemstone.ForEach(pair => pair.Key.ResonateInRing(who, (float)pair.Value));
        }

        who.MagneticRadius += this._magnetism;
        if (this._lightSource is null)
        {
            return;
        }

        location.removeLightSource(this._lightSource.Identifier);
        if (!who.leftRing.Value.IsCombinedInfinityBand(out _) && !who.rightRing.Value.IsCombinedInfinityBand(out _))
        {
            ModEntry.Events.Disable<ResonanceUpdateTickedEvent>();
        }

        if (this.Root is null)
        {
            return;
        }

        var tool = who.CurrentTool;
        if (tool is not (MeleeWeapon or Slingshot) || tool.enchantments.Count == 0 || !tool.HasEnchantmentOfType(this.Root))
        {
            return;
        }

        var count = tool.CountEnchantmentsOfType(this.Root);
        if (tool is MeleeWeapon weapon)
        {
            this.Root.DissonateInWeapon(weapon, (float)(this.Amplitude * count));
        }
        else if (tool is Slingshot slingshot)
        {
            this.Root.DissonateInSlingshot(slingshot, (float)(this.Amplitude * count));
        }
    }

    /// <summary>Adds resonance effects to the new <paramref name="location"/>.</summary>
    /// <param name="location">The new location.</param>
    public void OnNewLocation(GameLocation location)
    {
        if (this._lightSource is null)
        {
            return;
        }

        while (location.sharedLights.ContainsKey(this._lightSource.Identifier))
        {
            ++this._lightSource.Identifier;
        }

        location.sharedLights[this._lightSource.Identifier] = this._lightSource;
    }

    /// <summary>Advance the vibration phase by one stage.</summary>
    internal static void Vibrate()
    {
        _position = (_position + 1) % 120;
    }

    /// <summary>Removes resonance effects from the old <paramref name="location"/>.</summary>
    /// <param name="location">The left location.</param>
    internal void OnLeaveLocation(GameLocation location)
    {
        if (this._lightSource is null)
        {
            return;
        }

        location.removeLightSource(this._lightSource.Identifier);
    }

    /// <summary>Updates resonance effects.</summary>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    internal void Update(Farmer who)
    {
        this._phase = Range[_position] * Math.PI / 180d;
        if (this._lightSource is null)
        {
            return;
        }

        this._lightSource.radius.Value = this.GetLightSourceAmplitude();
        this._lightSource.color.Value = this.GetLightSourceColor();

        var offset = Vector2.Zero;
        if (who.shouldShadowBeOffset)
        {
            offset += who.drawOffset.Value;
        }

        this._lightSource.position.Value = new Vector2(who.Position.X + 32f, who.Position.Y + 32) + offset;
    }

    /// <summary>Adds the total resonance stat bonuses to the <paramref name="buffer"/>.</summary>
    /// <param name="buffer">A <see cref="StatBuffer"/> for aggregating stat bonuses.</param>
    internal void Buffer(StatBuffer buffer)
    {
        this.ResonanceByGemstone.ForEach(pair => pair.Key.Buffer(buffer, (float)pair.Value));
        buffer.AddedMagneticRadius += this._magnetism;
    }

    /// <summary>Evaluate the <see cref="HarmonicInterval"/>s between <see cref="Notes"/> and the resulting harmonies.</summary>
    private void Harmonize()
    {
        Array.Sort(this.Notes);
        var distinctNotes = this.Notes.Distinct().ToArray();

        // initialize resonances
        foreach (var note in distinctNotes)
        {
            this.ResonanceByGemstone[note] = 1d;
        }

        // octaves and unisons can be ignored
        if (distinctNotes.Length == 1)
        {
            return;
        }

        // add sequence intervals first
        var intervals = distinctNotes
            .Select((t, i) => new HarmonicInterval(
                t,
                distinctNotes[(i + 1) % distinctNotes.Length]))
            .ToList();

        // add composite intervals
        if (distinctNotes.Length >= 3)
        {
            intervals.AddRange(distinctNotes.Select((t, i) =>
                new HarmonicInterval(t, distinctNotes[(i + 2) % distinctNotes.Length])));
        }

        if (distinctNotes.Length >= 4)
        {
            intervals.AddRange(distinctNotes.Select((t, i) =>
                new HarmonicInterval(t, distinctNotes[(i + 3) % distinctNotes.Length])));
        }

        // determine root note
        var fifths = intervals
            .Where(i => i.Number == IntervalNumber.Fifth)
            .ToArray();
        if (fifths.Length is > 0 and < 3)
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
            this.Notes.ShiftUntilStartsWith(this.Root);
        }

        // group intervals
        this.GroupedIntervals = intervals
            .GroupBy(i => i.First)
            .ToArray();

        // evaluate total resonance of each note
        this.GroupedIntervals.ForEach(group =>
        {
            var numbers = group.Select(i => i.Number).ToHashSet();
            group.ForEach(i =>
            {
                // the perfect intervals
                if (i.Number == IntervalNumber.Fifth)
                {
                    this.ResonanceByGemstone[group.Key] += 1d / 2d;
                }

                if (i.Number == IntervalNumber.Fourth)
                {
                    this.ResonanceByGemstone[group.Key] += 1d / 3d;
                }

                // the consonant intervals
                if (i.Number == IntervalNumber.Third)
                {
                    if (this.Root is null || group.Key == this.Root)
                    {
                        this.ResonanceByGemstone[group.Key] += 1d / 5d;
                        if (this.Root is not null)
                        {
                            this._magnetism += 24;
                        }
                    }
                    else
                    {
                        this.ResonanceByGemstone[group.Key] += 1d / 6d;
                    }
                }

                if (i.Number == IntervalNumber.Sixth)
                {
                    if (this.Root is null || group.Key == this.Root)
                    {
                        this.ResonanceByGemstone[group.Key] += 1d / 5d;
                    }
                    else
                    {
                        this.ResonanceByGemstone[group.Key] += 1d / 6d;
                    }
                }

                // the dissonant intervals
                if (i.Number == IntervalNumber.Second)
                {
                    this.ResonanceByGemstone[group.Key] -= 1d / 8d;
                }

                if (i.Number == IntervalNumber.Seventh)
                {
                    if (numbers.Contains(IntervalNumber.Third) && numbers.Contains(IntervalNumber.Fifth))
                    {
                        this.ResonanceByGemstone[group.Key] += 1 / 8d;
                        this._magnetism += 24;
                    }
                    else
                    {
                        this.ResonanceByGemstone[group.Key] -= 1d / 8d;
                    }
                }
            });

            if (this.Root is not null)
            {
                this.Amplitude = this.ResonanceByGemstone[this.Root];
            }
        });
    }

    /// <summary>Initializes the <see cref="_lightSource"/> if a resonant harmony exists in the <see cref="Chord"/>.</summary>
    private void InitializeLightSource()
    {
        if (this.Root is null || this.Amplitude <= 1d)
        {
            return;
        }

        this._lightSource = new LightSource(
            ModEntry.Manifest.UniqueID.GetHashCode(),
            Vector2.Zero,
            (float)this.Amplitude,
            this.Root.InverseColor,
            playerID: Game1.player.UniqueMultiplayerID);
    }

    /// <summary>Evaluates the current amplitude of the <see cref="_lightSource"/>.</summary>
    /// <returns>The amplitude of the <see cref="_lightSource"/>.</returns>
    private float GetLightSourceAmplitude()
    {
        return (float)(this.Notes.Sum(n =>
            (1d + this.ResonanceByGemstone[n]) * Math.Sin(n.Frequency * this._phase)) / this.Amplitude);
    }

    /// <summary>Evaluates the current <see cref="Color"/> of the <see cref="_lightSource"/>.</summary>
    /// <returns>The <see cref="Color"/> of the <see cref="_lightSource"/>.</returns>
    private Color GetLightSourceColor()
    {
        return this.Root!.InverseColor;
    }
}
