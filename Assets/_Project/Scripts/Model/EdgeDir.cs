namespace HouseBuilder.Model
{
    /// <summary>
    /// Which edge of a cell an <see cref="EdgeId"/> refers to.
    /// Only North and East are stored, so every edge of the grid has exactly one id:
    /// the South edge of a cell is the North edge of the cell below it, the West edge
    /// is the East edge of the cell to its left.
    /// </summary>
    public enum EdgeDir
    {
        /// <summary>Edge on the +Z side of the cell, running along X.</summary>
        North,

        /// <summary>Edge on the +X side of the cell, running along Z.</summary>
        East
    }
}
