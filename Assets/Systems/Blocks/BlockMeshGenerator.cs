using System.Collections.Generic;
using UnityEngine;

public static class BlockMeshGenerator
{
    public static Mesh GenerateMesh(StandardBlocks standardBlocks)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uvs = new List<Vector2>();
        var uv2 = new List<Vector2>(); // To store the texture index

        var vertexOffset = 0;
        
        Vector3 scale = new Vector3(
            1f / standardBlocks.size.x,
            1f / standardBlocks.size.y,
            1f / standardBlocks.size.z
        );

        foreach (BlockData block in standardBlocks.blocks)
        {
            AddBlock(vertices, triangles, uvs, uv2, block, vertexOffset, standardBlocks, scale);
            vertexOffset += 24; // Each block has 24 vertices (6 faces, 4 vertices per face)
        }

        var mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray(),
            uv2 = uv2.ToArray()
        };

        mesh.RecalculateNormals();
        return mesh;
    }

    private static void AddBlock(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector2> uv2,
        BlockData block, int vertexOffset, StandardBlocks data, Vector3 scale)
    {
        var scaledPosition = Vector3.Scale(block.position, scale);

        Vector3[] faceVertices =
        {
            scaledPosition + Vector3.Scale(new Vector3(0, 1, 0), scale),
            scaledPosition + Vector3.Scale(new Vector3(1, 1, 0), scale),
            scaledPosition + Vector3.Scale(new Vector3(1, 1, 1), scale),
            scaledPosition + Vector3.Scale(new Vector3(0, 1, 1), scale), // Top
            scaledPosition + Vector3.Scale(new Vector3(0, 0, 1), scale),
            scaledPosition + Vector3.Scale(new Vector3(1, 0, 1), scale),
            scaledPosition + Vector3.Scale(new Vector3(1, 0, 0), scale),
            scaledPosition + Vector3.Scale(new Vector3(0, 0, 0), scale), // Bottom
            scaledPosition + Vector3.Scale(new Vector3(0, 1, 1), scale),
            scaledPosition + Vector3.Scale(new Vector3(1, 1, 1), scale),
            scaledPosition + Vector3.Scale(new Vector3(1, 0, 1), scale),
            scaledPosition + Vector3.Scale(new Vector3(0, 0, 1), scale), // Front
            scaledPosition + Vector3.Scale(new Vector3(1, 1, 0), scale),
            scaledPosition + Vector3.Scale(new Vector3(0, 1, 0), scale),
            scaledPosition + Vector3.Scale(new Vector3(0, 0, 0), scale),
            scaledPosition + Vector3.Scale(new Vector3(1, 0, 0), scale), // Back
            scaledPosition + Vector3.Scale(new Vector3(0, 1, 0), scale),
            scaledPosition + Vector3.Scale(new Vector3(0, 1, 1), scale),
            scaledPosition + Vector3.Scale(new Vector3(0, 0, 1), scale),
            scaledPosition + Vector3.Scale(new Vector3(0, 0, 0), scale), // Left
            scaledPosition + Vector3.Scale(new Vector3(1, 1, 1), scale),
            scaledPosition + Vector3.Scale(new Vector3(1, 1, 0), scale),
            scaledPosition + Vector3.Scale(new Vector3(1, 0, 0), scale),
            scaledPosition + Vector3.Scale(new Vector3(1, 0, 1), scale)  // Right
        };

        var uvMappings = new Dictionary<string, Vector4>
        {
            { "top", block.top.uv }, { "bottom", block.bottom.uv },
            { "front", block.front.uv }, { "back", block.back.uv },
            { "left", block.left.uv }, { "right", block.right.uv }
        };

        // Define face triangle indices
        int[] faceTriangles = { 0, 1, 2, 2, 3, 0 };

        // Check for adjacent blocks to cull internal faces
        string[] faces = { "top", "bottom", "front", "back", "left", "right" };
        Vector3Int[] faceOffsets =
        {
            new Vector3Int(0, 1, 0), // Top
            new Vector3Int(0, -1, 0), // Bottom
            new Vector3Int(0, 0, 1), // Front
            new Vector3Int(0, 0, -1), // Back
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(1, 0, 0) // Right
        };

        for (int i = 0; i < faces.Length; i++)
        {
            // Check if there's an adjacent block
            //if (!IsBlockAtPosition(block.position + faceOffsets[i], data))
            //{
                AddFace(vertices, triangles, uvs, uv2, faceVertices, faceTriangles, uvMappings[faces[i]], vertexOffset, i * 4, i);
            //}
        }
    }

    private static bool IsBlockAtPosition(Vector3Int position, StandardBlocks data)
    {
        foreach (var block in data.blocks)
        {
            if (block.position == position)
            {
                return true;
            }
        }

        return false;
    }

    private static void AddFace(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector2> uv2,
        Vector3[] faceVertices, int[] faceTriangles, Vector4 uvMapping, int vertexOffset, int startVertexIndex, int textureIndex)
    {
        vertices.Add(faceVertices[startVertexIndex + 3]);
        vertices.Add(faceVertices[startVertexIndex + 2]);
        vertices.Add(faceVertices[startVertexIndex + 1]);
        vertices.Add(faceVertices[startVertexIndex + 0]);

        for (var i = 0; i < faceTriangles.Length; i++)
        {
            triangles.Add(vertexOffset + startVertexIndex + faceTriangles[i]);
        }

        uvs.Add(new Vector2(uvMapping.x, uvMapping.y)); // Bottom-left
        uvs.Add(new Vector2(uvMapping.z, uvMapping.y)); // Bottom-right
        uvs.Add(new Vector2(uvMapping.z, uvMapping.w)); // Top-right
        uvs.Add(new Vector2(uvMapping.x, uvMapping.w)); // Top-left

        uv2.Add(new Vector2(textureIndex, 0)); // Bottom-left
        uv2.Add(new Vector2(textureIndex, 0)); // Bottom-right
        uv2.Add(new Vector2(textureIndex, 0)); // Top-right
        uv2.Add(new Vector2(textureIndex, 0)); // Top-left
    }
}