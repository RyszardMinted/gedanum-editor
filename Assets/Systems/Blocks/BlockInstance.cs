using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
    
public class BlockInstance : MonoBehaviour
{
    private static readonly int MainTexArray = Shader.PropertyToID("_MainTexArray");
    private static readonly int HighlightColor = Shader.PropertyToID("_HighlightColor");
    public StandardBlocks data; 

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        var originalMaterial = meshRenderer.material;
        var newMaterial = new Material(Shader.Find("Custom/BlockFaceHighlight"));
        newMaterial.SetTexture(MainTexArray, originalMaterial.GetTexture(MainTexArray));
        newMaterial.SetColor(HighlightColor, new Color(1, 1, 1, 0.5f));
        meshRenderer.material = newMaterial;

        // Add the face highlighter component
        gameObject.AddComponent<BlockFaceHighlighter>();
    }
    
    public void InitializeFromData(StandardBlocks blockData, BlockManager manager) {
        data = blockData;

        var mesh = BlockMeshGenerator.GenerateMesh(data, manager);
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh; 
        
        ApplyTextures(manager);
    }
    
    private void ApplyTextures(BlockManager manager) {
        Material material = meshRenderer.material;
        material.SetTexture("_MainTexArray", manager.textureArray);
    }
    
}
