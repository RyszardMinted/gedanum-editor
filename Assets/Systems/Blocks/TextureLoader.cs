using System.IO;
using UnityEngine;

public static class TextureLoader {
    public static Texture2D LoadTexture(string relativePath) {
        var resourcePath = Path.ChangeExtension(relativePath, null); 
        var texture = Resources.Load<Texture2D>(resourcePath);
        texture.filterMode = FilterMode.Point;

        if (texture == null) {
            Debug.LogError($"Texture not found in Resources: {resourcePath}");
        }

        return texture;
    }
    
    public static string LoadJson(string fileName) {
        var jsonFile = Resources.Load<TextAsset>(fileName);
        
        if (jsonFile == null) {
            Debug.LogError($"JSON file not found in Resources: {fileName}");
            return null;
        }
        return jsonFile.text;
    }
    
    public static Texture2DArray CreateTextureArray(string[] relativePaths) {
        if (relativePaths.Length == 0) {
            Debug.LogError("No texture paths provided.");
            return null;
        }

        var firstTexture = LoadTexture(relativePaths[0]);
        if (firstTexture == null) {
            Debug.LogError("Failed to load the first texture.");
            return null;
        }

        int width = firstTexture.width;
        int height = firstTexture.height;
        var format = firstTexture.format;

        var textureArray = new Texture2DArray(width, height, relativePaths.Length, format, false)
        {
            filterMode = FilterMode.Point
        };

        for (var i = 0; i < relativePaths.Length; i++) {
            var texture = LoadTexture(relativePaths[i]);
            if (texture == null) {
                Debug.LogWarning($"Skipping missing texture: {relativePaths[i]}");
                continue;
            }

            if (texture.width != width || texture.height != height) {
                Debug.LogError($"Texture size mismatch: {relativePaths[i]} does not match {width}x{height}");
                continue;
            }

            Graphics.CopyTexture(texture, 0, 0, textureArray, i, 0);
        }

        return textureArray;
    }
}