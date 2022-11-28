namespace DaLion.Ligo.Modules.Rings.Resonance;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using Microsoft.Xna.Framework;

#endregion using directives

/// <inheritdoc cref="IChord"/>
public sealed class Chord : IChord
{
    private static readonly double[] Range = Enumerable.Range(0, 360).Select(i => i * Math.PI / 180d).ToArray();

    private int _magnetism;
    private int _position;
    private double _phase;
    private double _period = 360d;
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
        who.Get_ResonatingChords().Add(this);

        this.ResonanceByGemstone.ForEach(pair => pair.Key.Resonate(who, (float)pair.Value));
        who.MagneticRadius += this._magnetism;
        if (this._lightSource is null)
        {
            return;
        }

        while (location.sharedLights.ContainsKey(this._lightSource.Identifier++))
        {
        }

        location.sharedLights[this._lightSource.Identifier] = this._lightSource;
    }

    /// <summary>Removes resonating stat bonuses from the farmer.</summary>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    public void Unapply(GameLocation location, Farmer who)
    {
        who.Get_ResonatingChords().Remove(this);

        this.ResonanceByGemstone.ForEach(pair => pair.Key.Dissonate(who, (float)pair.Value));
        who.MagneticRadius += this._magnetism;
        if (this._lightSource is null)
        {
            return;
        }

        location.removeLightSource(this._lightSource.Identifier);
    }

    /// <summary>Adds resonance effects to the new <paramref name="location"/>.</summary>
    /// <param name="location">The new location.</param>
    public void OnNewLocation(GameLocation location)
    {
        if (this._lightSource is null)
        {
            return;
        }

        while (location.sharedLights.ContainsKey(this._lightSource.Identifier++))
        {
        }

        location.sharedLights[this._lightSource.Identifier] = this._lightSource;
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
        if (this.Root is null)
        {
            return;
        }

        this._position = (int)((this._position + 1) % this._period);
        this._phase = Range[this._position];
        if (this._lightSource is null)
        {
            return;
        }

        this._lightSource.radius.Value = this.GetLightSourceRadius();
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
        buffer.MagneticRadius += this._magnetism;
    }

    /// <summary>Evaluate the <see cref="HarmonicInterval"/>s between <see cref="Notes"/> and the resulting harmonies.</summary>
    private void Harmonize()
    {
        Array.Sort(this.Notes);
        var distinctNotes = this.Notes.Distinct().ToArray();

        // initialize resonances
        foreach (var note in distinctNotes)
        {
            this.ResonanceByGemstone[note] = 0d;
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
                    return;
                }

                if (i.Number == IntervalNumber.Fourth)
                {
                    this.ResonanceByGemstone[group.Key] += 1d / 3d;
                    return;
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

                    return;
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

                    return;
                }

                // the dissonant intervals
                if (i.Number == IntervalNumber.Second)
                {
                    this.ResonanceByGemstone[group.Key] -= 1d / 8d;
                    return;
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
        });

        if (this.Notes.Length == 4 && distinctNotes.Length == 2 &&
            distinctNotes.All(d => this.Notes.Count(n => n == d) == 2))
        {
            this.ResonanceByGemstone.ForEach(pair => this.ResonanceByGemstone[pair.Key] *= 2f);
        }

        if (this.Root is not null)
        {
            this.Amplitude = this.ResonanceByGemstone[this.Root];
            this._period = 360d / this.Root.Frequency;
        }
    }

    /// <summary>Initializes the <see cref="_lightSource"/> if a resonant harmony exists in the <see cref="Chord"/>.</summary>
    private void InitializeLightSource()
    {
        if (this.Root is null)
        {
            return;
        }

        this._lightSource = new LightSource(
            LightSource.sconceLight,
            Vector2.Zero,
            (float)this.Amplitude,
            this.Root.GlowColor,
            playerID: Game1.player.UniqueMultiplayerID);
    }

    /// <summary>Evaluates the current amplitude of the <see cref="_lightSource"/>.</summary>
    /// <returns>The amplitude of the <see cref="_lightSource"/>.</returns>
    private float GetLightSourceRadius()
    {
        return (float)(this.Amplitude + (this.Amplitude / 10d * Math.Sin(this.Root!.Frequency * this._phase)));
    }

    /// <summary>Evaluates the current <see cref="Color"/> of the <see cref="_lightSource"/>.</summary>
    /// <returns>The <see cref="Color"/> of the <see cref="_lightSource"/>.</returns>
    private Color GetLightSourceColor()
    {
        return this.Root!.GlowColor;
    }
}
