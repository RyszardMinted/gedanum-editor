using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
    
public class BlockInstance : MonoBehaviour
{
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
