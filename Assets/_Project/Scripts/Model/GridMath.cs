using UnityEngine;

namespace HouseBuilder.Model
{
    /// <summary>
    /// Conversions between world space and the building grid.
    /// Cell (x, y) covers world X in [x, x+1) and world Z in [y, y+1) (for CellSize = 1).
    /// </summary>
    public static class GridMath
    {
        /// <summary>Cell that contains the given world point (height is ignored).</summary>
        public static Vector2Int WorldToCell(Vector3 world)
        {
            return new Vector2Int(
                Mathf.FloorToInt(world.x / GridSettings.CellSize),
                Mathf.FloorToInt(world.z / GridSettings.CellSize));
        }

        /// <summary>World position of the center of a cell at the given height.</summary>
        public static Vector3 CellCenter(Vector2Int cell, float height)
        {
            return new Vector3(
                (cell.x + 0.5f) * GridSettings.CellSize,
                height,
                (cell.y + 0.5f) * GridSettings.CellSize);
        }

        /// <summary>Height of the floor surface of a storey (0 = ground floor).</summary>
        public static float FloorElevation(int floor)
        {
            return floor * GridSettings.FloorHeight;
        }

        /// <summary>True if the cell lies on the plot.</summary>
        public static bool IsInsidePlot(Vector2Int cell)
        {
            return cell.x >= 0 && cell.y >= 0
                && cell.x < GridSettings.PlotSize && cell.y < GridSettings.PlotSize;
        }

        /// <summary>
        /// Edge of the grid closest to the given world point.
        /// The result is normalized to a North or East edge (see <see cref="EdgeDir"/>).
        /// </summary>
        public static EdgeId NearestEdge(Vector3 world)
        {
            Vector2Int cell = WorldToCell(world);
            float localX = world.x / GridSettings.CellSize - cell.x; // 0..1 inside the cell
            float localZ = world.z / GridSettings.CellSize - cell.y;

            float toWest = localX;
            float toEast = 1f - localX;
            float toSouth = localZ;
            float toNorth = 1f - localZ;

            float min = Mathf.Min(Mathf.Min(toWest, toEast), Mathf.Min(toSouth, toNorth));

            if (min == toNorth) return new EdgeId(cell, EdgeDir.North);
            if (min == toEast) return new EdgeId(cell, EdgeDir.East);
            if (min == toSouth) return new EdgeId(cell + Vector2Int.down, EdgeDir.North);
            return new EdgeId(cell + Vector2Int.left, EdgeDir.East);
        }

        /// <summary>World position of the middle of an edge at the given height.</summary>
        public static Vector3 EdgeCenter(EdgeId edge, float height)
        {
            float size = GridSettings.CellSize;
            return edge.Dir == EdgeDir.North
                ? new Vector3((edge.Cell.x + 0.5f) * size, height, (edge.Cell.y + 1f) * size)
                : new Vector3((edge.Cell.x + 1f) * size, height, (edge.Cell.y + 0.5f) * size);
        }

        /// <summary>Rotation that aligns an object's local X axis with the edge.</summary>
        public static Quaternion EdgeRotation(EdgeId edge)
        {
            return edge.Dir == EdgeDir.North ? Quaternion.identity : Quaternion.Euler(0f, 90f, 0f);
        }
    }
}
