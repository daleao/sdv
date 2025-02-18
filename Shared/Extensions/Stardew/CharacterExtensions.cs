﻿namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Extensions for the <see cref="Character"/> class.</summary>
public static class CharacterExtensions
{
    /// <summary>
    ///     Gets the squared pixel distance between this <paramref name="character"/> and the target <paramref name="position"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="position">The target position.</param>
    /// <returns>The squared pixel distance between <paramref name="character"/> and the <paramref name="position"/>.</returns>
    public static float SquaredPixelDistance(this Character character, Vector2 position)
    {
        var dx = character.Position.X - position.X;
        var dy = character.Position.Y - position.Y;
        return (dx * dx) + (dy * dy);
    }

    /// <summary>
    ///     Gets the squared tile distance between this <paramref name="character"/> and the target <paramref name="tile"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="tile">The target tile.</param>
    /// <returns>The squared tile distance between <paramref name="character"/> and the <paramref name="tile"/>.</returns>
    public static float SquaredTileDistance(this Character character, Vector2 tile)
    {
        var dx = character.Tile.X - tile.X;
        var dy = character.Tile.Y - tile.Y;
        return (dx * dx) + (dy * dy);
    }

    /// <summary>Gets the distance in tiles from this <paramref name="character"/> to the specified <paramref name="other"/>.</summary>
    /// <param name="character">The source <see cref="Character"/>.</param>
    /// <param name="other">The other <see cref="Character"/>.</param>
    /// <returns>The distance in tiles from this <paramref name="character"/> to the <paramref name="other"/>.</returns>
    public static int TileDistanceTo(this Character character, Character other)
    {
        return character.TilePoint.ManhattanDistance(other.TilePoint);
    }

    /// <summary>Gets the distance in tiles from this <paramref name="character"/> to the specified <paramref name="player"/>.</summary>
    /// <param name="character">The source <see cref="Character"/>.</param>
    /// <param name="player">The target <see cref="Farmer"/>.</param>
    /// <returns>The distance in tiles from this <paramref name="character"/> to the <paramref name="player"/>.</returns>
    public static int TileDistanceToPlayer(this Character character, Farmer? player = null)
    {
        player ??= Game1.player;
        return character.TileDistanceTo(player);
    }

    /// <summary>
    ///     Finds the closest tile from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="character"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate <see cref="Building"/>s, if already available.</param>
    /// <returns>The closest tile from among the specified <paramref name="candidates"/> to this <paramref name="character"/>.</returns>
    public static Vector2 GetClosestTile(this Character character, IEnumerable<Vector2> candidates)
    {
        var closest = character.Tile;
        var distanceToClosest = float.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = character.SquaredTileDistance(candidate);
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        return closest;
    }

    /// <summary>
    ///     Finds the closest target from among the specified <paramref name="candidates"/> to this
    ///     <paramref name="character"/>.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="T"/>s, if already available.</param>
    /// <param name="getPosition">A delegate to retrieve the pixel coordinates of <typeparamref name="T"/>.</param>
    /// <param name="distance">The actual tile distance to the closest candidate found.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The closest target from among the specified <paramref name="candidates"/> to this <paramref name="character"/>.</returns>
    public static T? GetClosest<T>(
        this Character character,
        IEnumerable<T> candidates,
        Func<T, Vector2> getPosition,
        out float distance,
        Func<T, bool>? predicate = null)
        where T : class
    {
        predicate ??= _ => true;
        candidates = candidates.Where(c => predicate(c));
        T? closest = null;
        var distanceToClosest = float.MaxValue;
        foreach (var candidate in candidates)
        {
            var distanceToThisCandidate = character.SquaredPixelDistance(getPosition(candidate));
            if (distanceToThisCandidate >= distanceToClosest)
            {
                continue;
            }

            closest = candidate;
            distanceToClosest = distanceToThisCandidate;
        }

        distance = distanceToClosest;
        return closest;
    }

    /// <summary>
    ///     Finds the closest <see cref="Building"/> to this
    ///     <paramref name="character"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate buildings, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Building"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static Building? GetClosestBuilding(
        this Character character,
        IEnumerable<Building>? candidates = null,
        Func<Building, bool>? predicate = null)
    {
        candidates ??= character.currentLocation.buildings;
        return character.GetClosest(candidates, b => b.GetBoundingBox().Center.ToVector2(), out _, predicate);
    }

    /// <summary>
    ///     Finds the closest <see cref="Building"/> of subtype <typeparamref name="TBuilding"/> to this
    ///     <paramref name="character"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TBuilding">A subtype of <see cref="Building"/>.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TBuilding"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Building"/> of type <typeparamref name="TBuilding"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static TBuilding? GetClosestBuilding<TBuilding>(
        this Character character,
        IEnumerable<TBuilding>? candidates = null,
        Func<TBuilding, bool>? predicate = null)
        where TBuilding : Building
    {
        candidates ??= character.currentLocation.buildings.OfType<TBuilding>();
        return character.GetClosest(candidates, b => b.GetBoundingBox().Center.ToVector2(), out _, predicate);
    }

    /// <summary>
    ///     Finds the closest <see cref="NPC"/> to this
    ///     <paramref name="character"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate characters, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Character"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static Character? GetClosestCharacter(
        this Character character,
        IEnumerable<Character>? candidates = null,
        Func<Character, bool>? predicate = null)
    {
        predicate ??= _ => true;
        candidates ??= character.currentLocation.characters;
        return character.GetClosest(
            candidates,
            c => c.Position,
            out _,
            c => !ReferenceEquals(c, character) && predicate(c));
    }

    /// <summary>
    ///     Finds the closest <see cref="Character"/> to this one in the current <see cref="GameLocation"/>, and of the
    ///     specified subtype.
    /// </summary>
    /// <typeparam name="TCharacter">A subtype of <see cref="Character"/>.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TCharacter"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Character"/> of type <typeparamref name="TCharacter"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static TCharacter? GetClosestCharacter<TCharacter>(
        this Character character,
        IEnumerable<TCharacter>? candidates = null,
        Func<TCharacter, bool>? predicate = null)
        where TCharacter : Character
    {
        predicate ??= _ => true;
        candidates ??= character.currentLocation.characters.OfType<TCharacter>();
        return character.GetClosest(
            candidates,
            c => c.Position,
            out _,
            c => !ReferenceEquals(c, character) && predicate(c));
    }

    /// <summary>
    ///     Find the closest <see cref="Farmer"/> to this <paramref name="character"/> in the current
    ///     <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate <see cref="Farmer"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="Farmer"/> with the minimal distance to <paramref name="character"/>.</returns>
    /// <remarks>This version is required as <see cref="Farmer"/> references are stored in a different field of <see cref="GameLocation"/>.</remarks>
    public static Farmer? GetClosestFarmer(
        this Character character,
        IEnumerable<Farmer>? candidates = null,
        Func<Farmer, bool>? predicate = null)
    {
        predicate ??= _ => true;
        candidates ??= character.currentLocation.farmers;
        return character.GetClosest(
            candidates,
            f => f.Position,
            out _,
            f => !ReferenceEquals(f, character) && predicate(f));
    }

    /// <summary>
    ///     Find the closest <see cref="SObject"/> to this
    ///     <paramref name="character"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate objects, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="SObject"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static SObject? GetClosestObject(
        this Character character,
        IEnumerable<SObject>? candidates = null,
        Func<SObject, bool>? predicate = null)
    {
        candidates ??= character.currentLocation.Objects.Values;
        return character.GetClosest(candidates, o => o.TileLocation * Game1.tileSize, out _, predicate);
    }

    /// <summary>
    ///     Find the closest <see cref="SObject"/> of subtype <typeparamref name="TObject"/> to this
    ///     <paramref name="character"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TObject">A subtype of <see cref="SObject"/>.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TObject"/>s, if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="SObject"/> of type <typeparamref name="TObject"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static TObject? GetClosestObject<TObject>(
        this Character character,
        IEnumerable<TObject>? candidates = null,
        Func<TObject, bool>? predicate = null)
        where TObject : SObject
    {
        candidates ??= character.currentLocation.Objects.Values.OfType<TObject>();
        return character.GetClosest(candidates, o => o.TileLocation * Game1.tileSize, out _, predicate);
    }

    /// <summary>
    ///     Find the closest <see cref="TerrainFeature"/> to this
    ///     <paramref name="character"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate terrain features if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="TerrainFeature"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static TerrainFeature? GetClosestTerrainFeature(
        this Character character,
        IEnumerable<TerrainFeature>? candidates = null,
        Func<TerrainFeature, bool>? predicate = null)
    {
        candidates ??= character.currentLocation.terrainFeatures.Values;
        return character.GetClosest(candidates, t => t.Tile * Game1.tileSize, out _, predicate);
    }

    /// <summary>
    ///     Find the closest <see cref="TerrainFeature"/> of subtype <typeparamref name="TTerrainFeature"/> to this
    ///     <paramref name="character"/> in the current <see cref="GameLocation"/>.
    /// </summary>
    /// <typeparam name="TTerrainFeature">A subtype of <see cref="TerrainFeature"/>.</typeparam>
    /// <param name="character">The <see cref="Character"/>.</param>
    /// <param name="candidates">The candidate <typeparamref name="TTerrainFeature"/>s if already available.</param>
    /// <param name="predicate">An optional condition with which to filter out candidates.</param>
    /// <returns>The <see cref="TerrainFeature"/> of type <typeparamref name="TTerrainFeature"/> with the minimal distance to <paramref name="character"/>.</returns>
    public static TTerrainFeature? GetClosestTerrainFeature<TTerrainFeature>(
        this Character character,
        IEnumerable<TTerrainFeature>? candidates = null,
        Func<TTerrainFeature, bool>? predicate = null)
        where TTerrainFeature : TerrainFeature
    {
        candidates ??= character.currentLocation.terrainFeatures.Values.OfType<TTerrainFeature>();
        return character.GetClosest(candidates, t => t.Tile * Game1.tileSize, out _, predicate);
    }
}
