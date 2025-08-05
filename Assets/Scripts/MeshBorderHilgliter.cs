using UnityEngine;

public class MeshBorderHighlighter : MonoBehaviour
{
    private MeshFilter meshFilter;
    private LineRenderer lineRenderer;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Set the LineRenderer properties
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.black;  // Color of the border
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        DrawBorder();
    }

    void DrawBorder()
    {
        // Get the mesh from the MeshFilter
        Mesh mesh = meshFilter.mesh;

        // Get the vertices of the mesh
        Vector3[] vertices = mesh.vertices;

        // Get the triangles (edges)
        int[] triangles = mesh.triangles;

        // Create an array to store the border vertices
        Vector3[] borderVertices = new Vector3[triangles.Length];

        // Fill in the border vertices by following the triangle edges
        for (int i = 0; i < triangles.Length; i++)
        {
            borderVertices[i] = vertices[triangles[i]];
        }

        // Set the LineRenderer positions to draw the border
        lineRenderer.positionCount = borderVertices.Length;
        lineRenderer.SetPositions(borderVertices);
    }
}
