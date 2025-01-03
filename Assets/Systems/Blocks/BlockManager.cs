using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class BlockManager : MonoBehaviour {
    [SerializeField] private GameObject blockPrefab;

    public void LoadBlocksFromJson(string jsonFileName) {
        var json = TextureLoader.LoadJson(jsonFileName);
        if (string.IsNullOrEmpty(json)) return;

        // Configure JSON deserialization settings
        var settings = new JsonSerializerSettings {
            Converters = new List<JsonConverter> { new Vector3IntConverter(), new Vector4Converter() }
        };

        // Deserialize the StandardBlocks structure
        var standardBlocks = JsonConvert.DeserializeObject<StandardBlocks>(json, settings);

        if (standardBlocks == null) {
            Debug.LogError("Failed to deserialize blocks.json");
            return;
        }

        // Create the block structure
        CreateBlockStructure(standardBlocks);
    }

    private void CreateBlockStructure(StandardBlocks standardBlocks) {
        // Instantiate a single GameObject to hold the entire block structure
        var blockObject = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);
        var blockInstance = blockObject.GetComponent<BlockInstance>();

        if (blockInstance == null) {
            Debug.LogError("The blockPrefab must have a BlockInstance component.");
            return;
        }

        // Initialize the BlockInstance with the entire StandardBlocks structure
        blockInstance.InitializeFromData(standardBlocks);
    }

    private void Start() {
        LoadBlocksFromJson("blocks");
    }
}
