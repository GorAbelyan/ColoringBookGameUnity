using UnityEngine;
using System.Collections.Generic;

 
public class MeshFiller : MonoBehaviour
    {
        public Material fillMaterial; // Material for the fill

        void Start()
        {
            Mesh mesh = GetComponent<MeshFilter>().mesh; // Get the existing mesh from the MeshFilter
            FillMesh(mesh); // Call the method to fill the mesh
        }
        void FillMesh(Mesh mesh)
        {
            // Get the current vertices of the mesh
            Vector3[] vertices = mesh.vertices;

            // Create a list of triangles to fill the inner area
            List<int> triangles = new List<int>();

            // Example: if you have a polygon with vertices already defined,
            // you will want to triangulate the inner area by connecting the center
            // to each edge of the polygon.

            // Assuming the polygon is a closed shape, add a center vertex (0, 0, 0)
            Vector3 center = Vector3.zero; // If you have a specific center, adjust this

            // Loop through the polygon vertices and create triangles
            for (int i = 1; i < vertices.Length - 1; i++)
            {
                triangles.Add(0); // Center vertex
                triangles.Add(i); // Current vertex
                triangles.Add(i + 1); // Next vertex
            }

            // Close the last triangle (optional, to connect the last vertex to the center)
            triangles.Add(0); // Center vertex
            triangles.Add(vertices.Length - 1); // Last vertex
            triangles.Add(1); // First vertex

            // Assign new triangles to the mesh (this will fill the inside)
            mesh.triangles = triangles.ToArray();

            // Recalculate normals for proper lighting/shading
            mesh.RecalculateNormals();

            // Optionally, recalculate bounds if needed
            mesh.RecalculateBounds();
        }
    } 
