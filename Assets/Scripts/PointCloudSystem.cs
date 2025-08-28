using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using UnityEngine.Rendering;
using System;
using SimpleFileBrowser;
using System.Collections;

public class PointCloudVisualizer : MonoBehaviour {
    [Header("Point Cloud Settings")]
    public Material pointMaterial;
    public float pointSize = 0.5f;
    public Color defaultPointColor = Color.white;
    public Color selectedPointColor = Color.red;
    public Color hoverPointColor = Color.yellow;

    [Header("Interaction Settings")]
    public LayerMask pointLayerMask = 1;
    public KeyCode gravityToggleKey = KeyCode.Z;
    public KeyCode deletePointKey = KeyCode.Delete;
    public KeyCode clearSelectionKey = KeyCode.LeftControl;

    [Header("File Settings")]
    public string plyFileName = "pointcloud.ply";
    public SaveConfirmationPopup confirmationPopup;

    public List<PointCloudPoint> points = new();
    private readonly HashSet<int> selectedPoints = new();
    private int hoveredPointIndex = -1;
    private Camera mainCamera;
    [Header("Gravity")]
    public bool gravityEnabled = false;

    [System.Serializable]
    public class PointCloudPoint {
        public Vector3 position;
        public Color color;
        public GameObject gameObject;
        public bool isSelected;
        public int index;

        public PointCloudPoint(Vector3 pos, Color col, int idx) {
            position = pos;
            color = col;
            index = idx;
            isSelected = false;
        }
    }

    void Start() {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();

        LoadPLYFile();
        CreatePointCloudVisualization();
    }

    void Update() {
        HandleInput();
        HandleMouseInteraction();
    }

    void HandleInput() {
 
        // Delete selected points
        if (Input.GetKeyDown(deletePointKey)) {
            DeleteSelectedPoints();
        }

        // Clear selection with Left-CTRL
        if (Input.GetKeyDown(clearSelectionKey)) {
            ClearSelection();
        }

        if (Input.GetKeyDown(gravityToggleKey)) { 
            ToggleGravity();
        }
    }

    void HandleMouseInteraction() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Reset hover state
        if (hoveredPointIndex >= 0 && hoveredPointIndex < points.Count) {
            UpdatePointVisual(hoveredPointIndex);
        }
        hoveredPointIndex = -1;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, pointLayerMask)) {
            if (hit.collider.TryGetComponent<PointRenderer>(out var pRender)) {
                PointCloudPoint point = pRender.point;
                if (point != null) {
                    hoveredPointIndex = point.index;
                    UpdatePointVisual(hoveredPointIndex, true);

                    // Handle clicking on points
                    if (Input.GetMouseButtonDown(0)) {
                        TogglePointSelection(point.index);
                    }
                }
            }
            
        }
        if (Input.GetMouseButtonDown(1)) {
            // Add new point at clicked position
            Vector3 worldPos = ray.origin + ray.direction * 10f; // 10 units from camera
            AddPoint(worldPos, defaultPointColor);
        }
    }

    void LoadPLYFile() {
        string filePath = plyFileName;

        if (!File.Exists(filePath)) {
            Debug.LogWarning($"PLY file not found at {filePath}. Creating sample points.");
            return;
        }

        try {
            string[] lines = File.ReadAllLines(filePath);
            bool inHeader = true;
            int vertexCount = 0;
            int currentVertex = 0;

            foreach (string line in lines) {
                if (inHeader) {
                    if (line.StartsWith("element vertex")) {
                        string[] parts = line.Split(' ');
                        if (parts.Length >= 3) {
                            int.TryParse(parts[2], out vertexCount);
                        }
                    }
                    else if (line == "end_header") {
                        inHeader = false;
                    }
                }
                else if (currentVertex < vertexCount) {
                    ParseVertexLine(line, currentVertex);
                    currentVertex++;
                }
            }

            Debug.Log($"Loaded {points.Count} points from PLY file");
        }
        catch (System.Exception e) {
            Debug.LogError($"Error loading PLY file: {e.Message}");
        }
    }

    void ParseVertexLine(string line, int index) {
        string[] parts = line.Split(' ');
        if (parts.Length >= 3) {
            if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z)) {
                Vector3 position = new(x, y, z);

                position += transform.position;
                Color color = defaultPointColor;

                // Parse color if available (assuming RGB values in parts[3], [4], [5])
                if (parts.Length >= 6) {
                    if (float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float r) &&
                        float.TryParse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture, out float g) &&
                        float.TryParse(parts[5], NumberStyles.Float, CultureInfo.InvariantCulture, out float b)) {
                        // Normalize color values if they're in 0-255 range
                        if (r > 1f || g > 1f || b > 1f) {
                            color = new Color(r / 255f, g / 255f, b / 255f, 1f);
                        }
                        else {
                            color = new Color(r, g, b, 1f);
                        }
                    }
                }

                points.Add(new PointCloudPoint(position, color, index));
            }
        }
    }

    void CreatePointCloudVisualization() {
        for (int i = 0; i < points.Count; i++) {
            CreatePointGameObject(i);
        }
    }

    void ToggleGravity() {
        gravityEnabled = !gravityEnabled;
        for (int i = 0; i < points.Count; i++) {
            Rigidbody singlePoint = points[i].gameObject.GetComponent<Rigidbody>();
            if (singlePoint != null) {
                singlePoint.useGravity = gravityEnabled;
                singlePoint.isKinematic = !gravityEnabled;
            }
            points[i].gameObject.transform.position = points[i].position;
        }
    }

    void CreatePointGameObject(int index) {
        if (index >= points.Count) return;

        PointCloudPoint point = points[index];

        // Create sphere primitive for the point

        GameObject pointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointObj.transform.position = point.position;
        pointObj.transform.localScale = Vector3.one * pointSize;
        pointObj.transform.parent = transform;
        pointObj.name = $"Point_{index}";
        
        Rigidbody rb = pointObj.AddComponent<Rigidbody>();
        rb.mass = 0.01f;                     
        rb.linearDamping = 0.1f;                      
        rb.angularDamping = 0.05f;             
        rb.useGravity = gravityEnabled;               
        rb.isKinematic = !gravityEnabled;
        //rb.detectCollisions = gravityEnabled;

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        pointObj.layer = Mathf.RoundToInt(Mathf.Log(pointLayerMask.value, 2));

        // Set up material
        Renderer renderer = pointObj.GetComponent<Renderer>();
        if (pointMaterial != null) {
            renderer.material = new Material(pointMaterial);
        }
        renderer.material.color = point.color;
        renderer.shadowCastingMode = ShadowCastingMode.Off;

        // Add point renderer component
        PointRenderer pointRenderer = pointObj.AddComponent<PointRenderer>();
        pointRenderer.point = point;

        point.gameObject = pointObj;
    }

    public void AddPoint(Vector3 position, Color color) {
        int newIndex = points.Count;
        PointCloudPoint newPoint = new(position, color, newIndex);
        points.Add(newPoint);
        CreatePointGameObject(newIndex);

        Debug.Log($"Added point at {position}");
    }

    void TogglePointSelection(int index) {
        if (selectedPoints.Contains(index)) {
            selectedPoints.Remove(index);
            points[index].isSelected = false;
        }
        else {
            selectedPoints.Add(index);
            points[index].isSelected = true;
        }
        UpdatePointVisual(index);
    }

    void ClearSelection() {
        foreach (int index in selectedPoints) {
            if (index < points.Count) {
                points[index].isSelected = false;
                UpdatePointVisual(index);
            }
        }
        selectedPoints.Clear();
    }

    public void SetNewColor(Color color) {
        for (int i = 0; i < points.Count; i++) {
            points[i].color = color;
            UpdatePointVisual(i);
        }
    }
    public void ResetColors() {
        for (int i = 0; i < points.Count; i++) {
            UpdatePointVisual(i);
        }
    }

    void UpdatePointVisual(int index, bool isHovered = false) {
        if (index < 0 || index >= points.Count) return;

        PointCloudPoint point = points[index];
        if (point.gameObject == null) return;

        Renderer renderer = point.gameObject.GetComponent<Renderer>();
        if (renderer == null) return;

        Color targetColor;
        if (isHovered) {
            targetColor = hoverPointColor;
        }
        else if (point.isSelected) {
            targetColor = selectedPointColor;
        }
        else {
            targetColor = point.color;
        }

        renderer.material.color = targetColor;

        UpdatePointSize(point);
    }

    private void UpdatePointSize(PointCloudPoint point) {
       if (point == null) return;
       float scale = point.isSelected ? pointSize * 1.2f : pointSize;
       point.gameObject.transform.localScale = Vector3.one * scale;
    }

    public void IncrementPointSize(float increment) {
        pointSize = pointSize + increment >= 0 ? pointSize + increment : 0;
        for (int i = 0; i < points.Count; i++) {
            UpdatePointSize(points[i]);
        }
    }

    public void resetPoints() {
        Debug.Log("Resetting points");
        for (int i = points.Count - 1; i >= 0; i--) {
            var pt = points[i];
            if (pt.gameObject != null)
                Destroy(pt.gameObject);
        }

        // 2) Svuota la lista
        points.Clear();
        points = new(); 
        LoadPLYFile();
        CreatePointCloudVisualization();
    }
    
    void DeleteSelectedPoints()
    {
        if (selectedPoints.Count == 0) return;
        
        List<int> indicesToDelete = new List<int>(selectedPoints);
        indicesToDelete.Sort((a, b) => b.CompareTo(a)); // Sort in descending order
        
        foreach (int index in indicesToDelete)
        {
            if (index < points.Count)
            {
                if (points[index].gameObject != null)
                {
                    DestroyImmediate(points[index].gameObject);
                }
                points.RemoveAt(index);
            }
        }
        
        // Update indices for remaining points
        for (int i = 0; i < points.Count; i++)
        {
            points[i].index = i;
            if (points[i].gameObject != null)
            {
                points[i].gameObject.name = $"Point_{i}";
                points[i].gameObject.GetComponent<PointRenderer>().point = points[i];
            }
        }
        
        selectedPoints.Clear();
        Debug.Log($"Deleted {indicesToDelete.Count} points");
    }

    public void SaveToPLY() {
        // 1) filtra solo .ply
        FileBrowser.SetFilters(true, new FileBrowser.Filter("PLY Files", ".ply"));
        FileBrowser.SetDefaultFilter(".ply");

        // 2) apri la save‐dialog
        FileBrowser.ShowSaveDialog(
            onSuccess: paths => StartCoroutine(OnPLYPathChosen(paths[0])),
            onCancel: () => Debug.Log("Save operation cancelled."),
            pickMode: FileBrowser.PickMode.Files,
            allowMultiSelection: false,
            initialPath: null,
            initialFilename: "cloud.ply",
            title: "Salva Point Cloud",
            saveButtonText: "Salva"
        );
    }

    private IEnumerator OnPLYPathChosen(string path) {
        yield return null;

        // assicura estensione
        if (Path.GetExtension(path).ToLower() != ".ply")
            path = Path.ChangeExtension(path, ".ply");

        // salva
        try {
            // crea folder se manca
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var writer = new StreamWriter(path)) {
                // Write PLY header
                writer.WriteLine("ply");
                writer.WriteLine("format ascii 1.0");
                writer.WriteLine($"element vertex {points.Count}");
                writer.WriteLine("property float x");
                writer.WriteLine("property float y");
                writer.WriteLine("property float z");
                writer.WriteLine("property uchar red");
                writer.WriteLine("property uchar green");
                writer.WriteLine("property uchar blue");
                writer.WriteLine("end_header");

                // Write vertex data
                foreach (PointCloudPoint point in points) {
                    string xs = point.position.x.ToString("F6", CultureInfo.InvariantCulture);
                    string ys = point.position.y.ToString("F6", CultureInfo.InvariantCulture);
                    string zs = point.position.z.ToString("F6", CultureInfo.InvariantCulture);
                    writer.WriteLine($"{xs} {ys} {zs} " +
                                   $"{(int)(point.color.r * 255)} {(int)(point.color.g * 255)} {(int)(point.color.b * 255)}");
                }
            }
            string message = $"Point cloud saved in:\n{path}";
            confirmationPopup.Show(message, 3f);
        }
        catch (Exception ex) {
            Debug.LogError($"Error saving PLY:\n{ex.GetType().Name}: {ex.Message}");
        }
    }
}

// Helper component to associate GameObjects with PointCloudPoint data
public class PointRenderer : MonoBehaviour
{
    public PointCloudVisualizer.PointCloudPoint point;
}