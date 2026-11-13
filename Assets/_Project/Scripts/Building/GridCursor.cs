using HouseBuilder.Model;
using UnityEngine;

namespace HouseBuilder.Building
{
    /// <summary>
    /// Casts a ray from the center of the screen onto the active storey's floor plane
    /// and reports the grid cell and the nearest edge the player is looking at.
    /// </summary>
    public class GridCursor : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _cellHighlight;
        [SerializeField] private Transform _edgeHighlight;
        [SerializeField] private float _maxDistance = 20f;

        // Lifts highlights slightly above the floor to avoid z-fighting.
        private const float HighlightOffset = 0.02f;

        private static readonly Vector3 ScreenCenter = new Vector3(0.5f, 0.5f, 0f);

        /// <summary>Storey the cursor works on (0 = ground floor).</summary>
        public int ActiveFloor { get; set; }

        /// <summary>True when the player looks at a cell on the plot within reach.</summary>
        public bool HasTarget { get; private set; }

        public Vector2Int Cell { get; private set; }
        public EdgeId Edge { get; private set; }
        public Vector3 HitPoint { get; private set; }

        private void Update()
        {
            UpdateTarget();
            UpdateHighlights();
        }

        private void UpdateTarget()
        {
            float elevation = GridMath.FloorElevation(ActiveFloor);
            Ray ray = _camera.ViewportPointToRay(ScreenCenter);
            var floorPlane = new Plane(Vector3.up, new Vector3(0f, elevation, 0f));

            if (!floorPlane.Raycast(ray, out float distance) || distance > _maxDistance)
            {
                HasTarget = false;
                return;
            }

            HitPoint = ray.GetPoint(distance);
            Cell = GridMath.WorldToCell(HitPoint);
            Edge = GridMath.NearestEdge(HitPoint);
            HasTarget = GridMath.IsInsidePlot(Cell);
        }

        private void UpdateHighlights()
        {
            _cellHighlight.gameObject.SetActive(HasTarget);
            _edgeHighlight.gameObject.SetActive(HasTarget);

            if (!HasTarget)
            {
                return;
            }

            float height = GridMath.FloorElevation(ActiveFloor) + HighlightOffset;

            _cellHighlight.position = GridMath.CellCenter(Cell, height);
            _edgeHighlight.SetPositionAndRotation(
                GridMath.EdgeCenter(Edge, height + HighlightOffset),
                GridMath.EdgeRotation(Edge));
        }
    }
}
