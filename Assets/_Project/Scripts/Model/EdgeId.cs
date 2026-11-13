using System;
using UnityEngine;

namespace HouseBuilder.Model
{
    /// <summary>
    /// Unique id of one edge of the grid: a cell plus its North or East edge.
    /// Walls, windows and doors are placed on edges.
    /// </summary>
    public readonly struct EdgeId : IEquatable<EdgeId>
    {
        public Vector2Int Cell { get; }
        public EdgeDir Dir { get; }

        public EdgeId(Vector2Int cell, EdgeDir dir)
        {
            Cell = cell;
            Dir = dir;
        }

        public bool Equals(EdgeId other) => Cell == other.Cell && Dir == other.Dir;

        public override bool Equals(object obj) => obj is EdgeId other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Cell, (int)Dir);

        public static bool operator ==(EdgeId a, EdgeId b) => a.Equals(b);

        public static bool operator !=(EdgeId a, EdgeId b) => !a.Equals(b);

        public override string ToString() => $"Edge({Cell.x}, {Cell.y}, {Dir})";
    }
}
