namespace HouseBuilder.Model
{
    /// <summary>
    /// Global grid and building dimensions. The single place for these numbers.
    /// </summary>
    public static class GridSettings
    {
        /// <summary>Size of one grid cell in meters.</summary>
        public const float CellSize = 1f;

        /// <summary>Height of one storey in meters.</summary>
        public const float FloorHeight = 3f;

        /// <summary>Number of storeys the player can build on.</summary>
        public const int MaxFloors = 2;

        /// <summary>Plot size in cells along X and Z. The plot spans cells [0, PlotSize).</summary>
        public const int PlotSize = 50;
    }
}
