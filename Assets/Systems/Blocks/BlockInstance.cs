using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
    
public class BlockInstance : MonoBehaviour
{
    public StandardBlocks data; 

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }
    
    public void InitializeFromData(StandardBlocks blockData) {
        data = blockData;

        var mesh = BlockMeshGenerator.GenerateMesh(data);
        meshFilter.mesh = mesh;
        
        ApplyTextures(data);
    }
    
    private void ApplyTextures(StandardBlocks blockData) {
        var texturePaths = new HashSet<string>();

        foreach (var block in blockData.blocks) {
            texturePaths.Add($"Textures/{block.top.texture}");
            texturePaths.Add($"Textures/{block.bottom.texture}");
            texturePaths.Add($"Textures/{block.front.texture}");
            texturePaths.Add($"Textures/{block.back.texture}");
            texturePaths.Add($"Textures/{block.left.texture}");
            texturePaths.Add($"Textures/{block.right.texture}");
        }

        var textureArray = TextureLoader.CreateTextureArray(texturePaths.ToArray());
        if (textureArray == null) {
            Debug.LogError("Failed to create Texture2DArray.");
            return;
        }

        Material material = meshRenderer.material;
        material.SetTexture("_MainTexArray", textureArray);
    }
    
}
