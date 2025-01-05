using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class BlockManager : MonoBehaviour {
    [SerializeField] private GameObject blockPrefab;

    public StandardBlocks LoadInnerBlock(string path)
    {
        var json = File.ReadAllText(path);
        
        if (string.IsNullOrEmpty(json)) return null;

        var settings = new JsonSerializerSettings {
            Converters = new List<JsonConverter> { new Vector3IntConverter(), new Vector4Converter() }
        };

        var standardBlocks = JsonConvert.DeserializeObject<StandardBlocks>(json, settings);

        if (standardBlocks == null) {
            Debug.LogError("Failed to deserialize blocks.json");
            return null;
        }

        return standardBlocks;
    }
    
    public BlockInstance LoadBlocksFromJson(string jsonFileName) {
        var json = File.ReadAllText(jsonFileName);
        if (string.IsNullOrEmpty(json)) return null;

        var settings = new JsonSerializerSettings {
            Converters = new List<JsonConverter> { new Vector3IntConverter(), new Vector4Converter() }
        };

        var standardBlocks = JsonConvert.DeserializeObject<StandardBlocks>(json, settings);

        if (standardBlocks == null) {
            Debug.LogError("Failed to deserialize blocks.json");
            return null;
        }

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

    public  BlockInstance InstantiateEmptyBlock()
    {
        var blockObject = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
        var blockInstance = blockObject.GetComponent<BlockInstance>();
        if (blockInstance == null) {
            Debug.LogError("The blockPrefab must have a BlockInstance component.");
            return null;
        }
        return blockInstance;
    }

    private BlockInstance CreateBlockStructure(StandardBlocks standardBlocks) {
        // Instantiate a single GameObject to hold the entire block structure
        var blockObject = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
        var blockInstance = blockObject.GetComponent<BlockInstance>();

        if (blockInstance == null) {
            Debug.LogError("The blockPrefab must have a BlockInstance component.");
            return null;
        }

        // Initialize the BlockInstance with the entire StandardBlocks structure
        blockInstance.InitializeFromData(standardBlocks);

        return blockInstance;
    }

    private void Start() {
        // LoadBlocksFromJson("blocks");
    }
}
