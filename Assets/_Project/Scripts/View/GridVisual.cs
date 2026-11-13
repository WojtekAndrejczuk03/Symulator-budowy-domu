using HouseBuilder.Model;
using UnityEngine;

namespace HouseBuilder.View
{
    /// <summary>
    /// Draws the plot grid as a line mesh (one line per cell border).
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class GridVisual : MonoBehaviour
    {
        [SerializeField] private float _heightOffset = 0.01f;

        private Mesh _mesh;

        private void OnEnable()
        {
            BuildMesh();
        }

        private void OnDisable()
        {
            if (_mesh == null)
            {
                return;
            }

            if (Application.isPlaying) Destroy(_mesh);
            else DestroyImmediate(_mesh);
        }

        private void BuildMesh()
        {
            int size = GridSettings.PlotSize;
            float length = size * GridSettings.CellSize;
            int lineCount = (size + 1) * 2;

            var vertices = new Vector3[lineCount * 2];
            var indices = new int[lineCount * 2];
            int v = 0;

            for (int i = 0; i <= size; i++)
            {
                float offset = i * GridSettings.CellSize;

                // Line along Z at x = offset.
                vertices[v++] = new Vector3(offset, _heightOffset, 0f);
                vertices[v++] = new Vector3(offset, _heightOffset, length);

                // Line along X at z = offset.
                vertices[v++] = new Vector3(0f, _heightOffset, offset);
                vertices[v++] = new Vector3(length, _heightOffset, offset);
            }

            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            _mesh = new Mesh { name = "PlotGrid", hideFlags = HideFlags.DontSave };
            _mesh.SetVertices(vertices);
            _mesh.SetIndices(indices, MeshTopology.Lines, 0);
            _mesh.RecalculateBounds();

            GetComponent<MeshFilter>().sharedMesh = _mesh;
        }
    }
}
