using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class BlockManager : MonoBehaviour {
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int blockLayer = 0; 

    [HideInInspector] [DoNotSerialize] public Texture2DArray textureArray;
    [HideInInspector] [DoNotSerialize] public List<string> textureNames;

    private void Awake()
    {
        LoadTextures();
    }

    public StandardBlocks LoadInnerBlock(string path)
    {
        var json = File.ReadAllText(path);
        
        if (string.IsNullOrEmpty(json)) return null;

        var settings = new JsonSerializerSettings {
            Converters = new List<JsonConverter> { new Vector3IntConverter(), new Vector4Converter() }
        };

        var standardBlocks = JsonConvert.DeserializeObject<StandardBlocks>(json, settings);
        standardBlocks.OwnerFilename = path;

        return standardBlocks;
    }
    
    public BlockInstance LoadBlocksFromJson(string jsonFileName) {
        var json = File.ReadAllText(jsonFileName);
        if (string.IsNullOrEmpty(json)) return null;

        var settings = new JsonSerializerSettings {
            Converters = new List<JsonConverter> { new Vector3IntConverter(), new Vector4Converter() }
        };

        var standardBlocks = JsonConvert.DeserializeObject<StandardBlocks>(json, settings);
        standardBlocks.OwnerFilename = jsonFileName;

        return CreateBlockStructure(standardBlocks);
    }

    public void SaveBlockToJson(string jsonFileName, BlockInstance block)
    {
        var settings = new JsonSerializerSettings {
            Converters = new List<JsonConverter> { new Vector3IntConverter(), new Vector4Converter() }
        };
        
        var path = Path.Combine(Application.persistentDataPath, $"{block.data.blockName}.json");
        var json = JsonConvert.SerializeObject(block.data, settings);
        
        File.WriteAllText(path, json);
    }

    public void DestroyProject(GameObject go)
    {
        Destroy(go);
    }

    public BlockInstance InstantiateEmptyBlock()
    {
        var blockObject = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
        var blockInstance = blockObject.GetComponent<BlockInstance>();
        if (blockInstance == null) {
            Debug.LogError("The blockPrefab must have a BlockInstance component.");
            return null;
        }
        
        blockObject.layer = blockLayer;
        
        return blockInstance;
    }

    private BlockInstance CreateBlockStructure(StandardBlocks standardBlocks) {
        var blockObject = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
        var blockInstance = blockObject.GetComponent<BlockInstance>();

        if (blockInstance == null) {
            Debug.LogError("The blockPrefab must have a BlockInstance component.");
            return null;
        }

        blockObject.layer = blockLayer;

        blockInstance.InitializeFromData(standardBlocks, this);

        return blockInstance;
    }

    public uint IndexOfTexture(string textureName)
    {
        for (var i = 0; i < textureNames.Count; i++)
        {
            if (textureNames[i] == textureName) return (uint) i;
        }

        return 0;
    }
    
    private void LoadTextures() {
        var texturePaths = new HashSet<string>();
        textureNames = new List<string>();

        var textures = Resources.LoadAll<Texture2D>("Textures");
        foreach (var texture in textures)
        {
            texturePaths.Add($"Textures/{texture.name}");
            textureNames.Add(texture.name);
            Debug.Log($"Loading texture {texture.name}");
        }
        
        textureArray = TextureLoader.CreateTextureArray(texturePaths.ToArray());
        if (textureArray == null) {
            Debug.LogError("Failed to create Texture2DArray.");
            return;
        }
    }

    private void Start() {
        // LoadBlocksFromJson("blocks");
    }
}
