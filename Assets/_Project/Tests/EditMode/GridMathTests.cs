using HouseBuilder.Model;
using NUnit.Framework;
using UnityEngine;

namespace HouseBuilder.Tests
{
    public class GridMathTests
    {
        [TestCase(0.0f, 0.0f, 0, 0)]
        [TestCase(0.99f, 0.99f, 0, 0)]
        [TestCase(1.0f, 2.5f, 1, 2)]
        [TestCase(-0.1f, -0.1f, -1, -1)]
        [TestCase(49.9f, 0.5f, 49, 0)]
        public void WorldToCell_ReturnsContainingCell(float x, float z, int expectedX, int expectedY)
        {
            Vector2Int cell = GridMath.WorldToCell(new Vector3(x, 123f, z));

            Assert.AreEqual(new Vector2Int(expectedX, expectedY), cell);
        }

        [Test]
        public void CellCenter_IsInMiddleOfCellAtGivenHeight()
        {
            Vector3 center = GridMath.CellCenter(new Vector2Int(3, 7), 3f);

            Assert.AreEqual(new Vector3(3.5f, 3f, 7.5f), center);
        }

        [Test]
        public void CellCenter_RoundTripsThroughWorldToCell()
        {
            var cell = new Vector2Int(12, 34);

            Assert.AreEqual(cell, GridMath.WorldToCell(GridMath.CellCenter(cell, 0f)));
        }

        [TestCase(0, 0f)]
        [TestCase(1, 3f)]
        public void FloorElevation_IsFloorTimesFloorHeight(int floor, float expected)
        {
            Assert.AreEqual(expected, GridMath.FloorElevation(floor));
        }

        [TestCase(0, 0, true)]
        [TestCase(49, 49, true)]
        [TestCase(-1, 0, false)]
        [TestCase(0, 50, false)]
        [TestCase(50, 10, false)]
        public void IsInsidePlot_ChecksPlotBounds(int x, int y, bool expected)
        {
            Assert.AreEqual(expected, GridMath.IsInsidePlot(new Vector2Int(x, y)));
        }

        [Test]
        public void NearestEdge_NearNorthSide_ReturnsNorthEdgeOfSameCell()
        {
            EdgeId edge = GridMath.NearestEdge(new Vector3(5.5f, 0f, 5.9f));

            Assert.AreEqual(new EdgeId(new Vector2Int(5, 5), EdgeDir.North), edge);
        }

        [Test]
        public void NearestEdge_NearEastSide_ReturnsEastEdgeOfSameCell()
        {
            EdgeId edge = GridMath.NearestEdge(new Vector3(5.9f, 0f, 5.5f));

            Assert.AreEqual(new EdgeId(new Vector2Int(5, 5), EdgeDir.East), edge);
        }

        [Test]
        public void NearestEdge_NearSouthSide_ReturnsNorthEdgeOfCellBelow()
        {
            EdgeId edge = GridMath.NearestEdge(new Vector3(5.5f, 0f, 5.1f));

            Assert.AreEqual(new EdgeId(new Vector2Int(5, 4), EdgeDir.North), edge);
        }

        [Test]
        public void NearestEdge_NearWestSide_ReturnsEastEdgeOfCellToTheLeft()
        {
            EdgeId edge = GridMath.NearestEdge(new Vector3(5.1f, 0f, 5.5f));

            Assert.AreEqual(new EdgeId(new Vector2Int(4, 5), EdgeDir.East), edge);
        }

        [Test]
        public void EdgeCenter_NorthEdge_IsOnTopBorderOfCell()
        {
            var edge = new EdgeId(new Vector2Int(2, 3), EdgeDir.North);

            Assert.AreEqual(new Vector3(2.5f, 0f, 4f), GridMath.EdgeCenter(edge, 0f));
        }

        [Test]
        public void EdgeCenter_EastEdge_IsOnRightBorderOfCell()
        {
            var edge = new EdgeId(new Vector2Int(2, 3), EdgeDir.East);

            Assert.AreEqual(new Vector3(3f, 0f, 3.5f), GridMath.EdgeCenter(edge, 0f));
        }

        [Test]
        public void EdgeId_SameCellAndDir_AreEqual()
        {
            var a = new EdgeId(new Vector2Int(1, 2), EdgeDir.East);
            var b = new EdgeId(new Vector2Int(1, 2), EdgeDir.East);

            Assert.IsTrue(a == b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
