using UnityEngine;
using Unity.VectorGraphics;
using System.Collections.Generic;
using System.IO;

public class SVGContourMeshBuilder : MonoBehaviour
{
    public string svgFileName = "animal_1"; // No extension
    public float scaleFactor = 0.01f; // Convert SVG units to Unity units
    public float zOffset = 0f;
    public Color svgLineColor = Color.black;
    public Color backgroundColor = Color.white;
    public Vector2 backgroundPadding = new Vector2(0.5f, 0.5f);


    Scene _scene;
    void Start()
    {
        Scene scene = LoadSVGScene(svgFileName);
        if (scene == null)
        {
            Debug.LogError("Failed to load SVG.");
            return;
        }

        // Center origin at (0,0)
        Rect svgBounds = VectorUtils.SceneNodeBounds(scene.Root);
        Vector2 svgCenter = svgBounds.center;


     //   DrawBackgroundQuad(svgBounds);

        var tessOptions = new VectorUtils.TessellationOptions
        {
            StepDistance = 1f,
            MaxCordDeviation = 0.5f,
            MaxTanAngleDeviation = 0.1f,
            SamplingStepSize = 0.01f
        };

        int contourIndex = 0;
        TraverseScene(scene.Root, tessOptions, svgCenter, ref contourIndex);
    }

    Scene LoadSVGScene(string fileName)
    {
        TextAsset svgFile = Resources.Load<TextAsset>(fileName);
        if (svgFile == null)
        {
            Debug.LogError("SVG file not found in Resources: " + fileName);
            return null;
        }

        return SVGParser.ImportSVG(new StringReader(svgFile.text)).Scene;
    }

    void TraverseScene(SceneNode node, VectorUtils.TessellationOptions tessOptions, Vector2 svgCenter, ref int index)
    {
        if (node.Shapes != null)
        {
            foreach (var shape in node.Shapes)
            {
                if (shape.Contours == null || shape.Contours.Length == 0)
                    continue;

                var pathProps = shape.PathProps;
                if (pathProps.Stroke == null)
                    pathProps.Stroke = new Stroke();

                foreach (var contour in shape.Contours)
                {
                    VectorUtils.TessellatePath(contour, pathProps, tessOptions, out var vertices, out var indices);
                    Mesh mesh = BuildMesh(vertices, indices, svgCenter);
                    CreateMeshObject(mesh, index);
                    index++;
                }
            }
        }

        if (node.Children != null)
        {
            foreach (var child in node.Children)
                TraverseScene(child, tessOptions, svgCenter, ref index);
        }
    }

    Mesh BuildMesh(Vector2[] vertices, ushort[] indices, Vector2 svgCenter)
    {
        Mesh mesh = new Mesh();
        Vector3[] verts3D = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            // Shift to center and scale down to fit camera view
            var centered = vertices[i] - svgCenter;
            verts3D[i] = new Vector3(centered.x * scaleFactor, centered.y * scaleFactor, zOffset);
        }

        int[] triangles = new int[indices.Length];
        for (int i = 0; i < indices.Length; i++)
            triangles[i] = indices[i];

        mesh.vertices = verts3D;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    void CreateMeshObject(Mesh mesh, int index)
    {
        GameObject go = new GameObject("ContourMesh_" + index);
        go.transform.SetParent(transform);
        var filter = go.AddComponent<MeshFilter>();
        var renderer = go.AddComponent<MeshRenderer>();

        go.transform.localRotation = Quaternion.Euler(0f, 0, 180f);
      

        filter.mesh = mesh;

        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = Color.red;
        renderer.material = mat;
    }
    void DrawBackgroundQuad(Rect svgBounds)
    {
        // Convert to Unity units
        Vector2 size = new Vector2(svgBounds.size.x, svgBounds.size.y) * scaleFactor + backgroundPadding;
        Vector3 center = new Vector3(svgBounds.center.x * scaleFactor, svgBounds.center.y * scaleFactor, zOffset - 0.1f); // Put behind lines

        GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bg.name = "SVG_Background";
        bg.transform.SetParent(transform);
        bg.transform.position = center;
        bg.transform.localScale = new Vector3(size.x, size.y, 1f);

        var mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = backgroundColor;

        var renderer = bg.GetComponent<MeshRenderer>();
        renderer.material = mat;
    }
}
