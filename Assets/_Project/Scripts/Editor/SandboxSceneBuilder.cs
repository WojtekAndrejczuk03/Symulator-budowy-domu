using HouseBuilder.Building;
using HouseBuilder.Model;
using HouseBuilder.Player;
using HouseBuilder.View;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HouseBuilder.EditorTools
{
    /// <summary>
    /// Builds the sandbox test scene from code, so scene files never have to be edited by hand.
    /// Menu: HouseBuilder → Utwórz scenę testową.
    /// </summary>
    public static class SandboxSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/Sandbox.unity";
        private const string MaterialsFolder = "Assets/_Project/Materials";
        private const string InputActionsPath = "Assets/InputSystem_Actions.inputactions";

        private const string LitShader = "Universal Render Pipeline/Lit";
        private const string UnlitShader = "Universal Render Pipeline/Unlit";

        // Ground extends beyond the plot so the player does not fall off the edge.
        private const float GroundMargin = 10f;
        private const float PlayerHeight = 1.8f;
        private const float PlayerRadius = 0.3f;
        private const float EyeHeight = 1.65f;
        private const float EdgeMarkerThickness = 0.08f;

        [MenuItem("HouseBuilder/Utwórz scenę testową")]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null &&
                !EditorUtility.DisplayDialog("Scena testowa",
                    $"Scena {ScenePath} już istnieje. Zbudować ją od nowa?", "Tak", "Anuluj"))
            {
                return;
            }

            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
            if (inputActions == null)
            {
                Debug.LogError($"Brak pliku akcji wejścia: {InputActionsPath}");
                return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            Object.DestroyImmediate(Camera.main.gameObject); // the player brings its own camera

            float plotLength = GridSettings.PlotSize * GridSettings.CellSize;
            var plotCenter = new Vector3(plotLength / 2f, 0f, plotLength / 2f);

            CreateGround(plotCenter, plotLength);
            CreateGrid();
            Camera playerCamera = CreatePlayer(inputActions, plotCenter);
            CreateGridCursor(playerCamera);
            CreateCrosshair();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings();

            Debug.Log($"Scena testowa gotowa: {ScenePath}");
        }

        private static void CreateGround(Vector3 plotCenter, float plotLength)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";

            // Unity's plane primitive is 10 × 10 m.
            float scale = (plotLength + GroundMargin * 2f) / 10f;
            ground.transform.position = plotCenter;
            ground.transform.localScale = new Vector3(scale, 1f, scale);
            ground.GetComponent<MeshRenderer>().sharedMaterial =
                GetOrCreateMaterial("Ground", LitShader, new Color(0.36f, 0.55f, 0.27f));
        }

        private static void CreateGrid()
        {
            var grid = new GameObject("PlotGrid", typeof(MeshFilter), typeof(MeshRenderer), typeof(GridVisual));
            var renderer = grid.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = GetOrCreateMaterial("GridLines", UnlitShader, new Color(0.2f, 0.32f, 0.16f));
            renderer.shadowCastingMode = ShadowCastingMode.Off;
        }

        private static Camera CreatePlayer(InputActionAsset inputActions, Vector3 plotCenter)
        {
            var player = new GameObject("Player");
            player.transform.position = new Vector3(plotCenter.x, 0.1f, 5f);

            var controller = player.AddComponent<CharacterController>();
            controller.height = PlayerHeight;
            controller.radius = PlayerRadius;
            controller.center = new Vector3(0f, PlayerHeight / 2f, 0f);

            var cameraObject = new GameObject("PlayerCamera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(player.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, EyeHeight, 0f);

            var camera = cameraObject.AddComponent<Camera>();
            camera.nearClipPlane = 0.05f;
            cameraObject.AddComponent<AudioListener>();

            var fps = player.AddComponent<FirstPersonController>();
            var serialized = new SerializedObject(fps);
            serialized.FindProperty("_actions").objectReferenceValue = inputActions;
            serialized.FindProperty("_cameraPivot").objectReferenceValue = cameraObject.transform;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            return camera;
        }

        private static void CreateGridCursor(Camera playerCamera)
        {
            var cursorRoot = new GameObject("GridCursor");

            GameObject cellHighlight = CreateMarker(PrimitiveType.Quad, "CellHighlight", cursorRoot.transform,
                GetOrCreateMaterial("CellHighlight", UnlitShader, new Color(1f, 0.85f, 0.2f)));
            cellHighlight.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            float cellVisualSize = GridSettings.CellSize * 0.9f;
            cellHighlight.transform.localScale = new Vector3(cellVisualSize, cellVisualSize, 1f);

            GameObject edgeHighlight = CreateMarker(PrimitiveType.Cube, "EdgeHighlight", cursorRoot.transform,
                GetOrCreateMaterial("EdgeHighlight", UnlitShader, new Color(1f, 0.45f, 0.1f)));
            edgeHighlight.transform.localScale =
                new Vector3(GridSettings.CellSize, EdgeMarkerThickness, EdgeMarkerThickness);

            var gridCursor = cursorRoot.AddComponent<GridCursor>();
            var serialized = new SerializedObject(gridCursor);
            serialized.FindProperty("_camera").objectReferenceValue = playerCamera;
            serialized.FindProperty("_cellHighlight").objectReferenceValue = cellHighlight.transform;
            serialized.FindProperty("_edgeHighlight").objectReferenceValue = edgeHighlight.transform;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject CreateMarker(PrimitiveType type, string name, Transform parent, Material material)
        {
            GameObject marker = GameObject.CreatePrimitive(type);
            marker.name = name;
            marker.transform.SetParent(parent, false);
            Object.DestroyImmediate(marker.GetComponent<Collider>());

            var renderer = marker.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            return marker;
        }

        private static void CreateCrosshair()
        {
            var canvasObject = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler));
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            var crosshair = new GameObject("Crosshair", typeof(RectTransform));
            crosshair.transform.SetParent(canvasObject.transform, false);

            CreateCrosshairBar(crosshair.transform, "Horizontal", new Vector2(16f, 2f));
            CreateCrosshairBar(crosshair.transform, "Vertical", new Vector2(2f, 16f));
        }

        private static void CreateCrosshairBar(Transform parent, string name, Vector2 size)
        {
            var bar = new GameObject(name, typeof(RectTransform), typeof(Image));
            bar.transform.SetParent(parent, false);
            bar.GetComponent<RectTransform>().sizeDelta = size;

            var image = bar.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.85f);
            image.raycastTarget = false;
        }

        private static Material GetOrCreateMaterial(string name, string shaderName, Color color)
        {
            string path = $"{MaterialsFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
            {
                return material;
            }

            Shader shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogError($"Nie znaleziono shadera: {shaderName}");
                shader = Shader.Find("Standard");
            }

            material = new Material(shader);
            material.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void AddSceneToBuildSettings()
        {
            foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
            {
                if (existing.path == ScenePath)
                {
                    return;
                }
            }

            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes)
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
