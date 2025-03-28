using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceBlockAction : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private LayerMask placementLayer;
    [SerializeField] private LayerMask selectionLayer;
    [SerializeField] private BlockEditorUIManager blockEditorUI;
    [SerializeField] private BlockFaceHighlighter highlighter;

    private VoxelEditorInput input;
    private ProjectManager projectManager;
    private BlockManager blockManager;
    
    // Default texture for new blocks
    private string defaultTexture = "stone";
    private Vector4 defaultUV = new Vector4(0, 0, 1, 1);

    public void Initialize(ProjectManager manager, BlockManager blockMng)
    {
        projectManager = manager;
        blockManager = blockMng;
    }
    
    public void SetInput(VoxelEditorInput inputToSet)
    {
        input = inputToSet;
        
        // Subscribe to selection input
        input.Editor.SelectBlock.performed += ctx => SelectBlock();
    }

    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayer))
        {
            var hitPoint = hit.point + hit.normal * 0.001f; 
            var subGrid = SnapToSubGrid(hitPoint, projectManager.GetGridSize());

            var block = highlighter.GetSelectedBlock();
            if (block == null)
            {
                projectManager.DebugText = subGrid.ToString();
            }
            else
            {
                subGrid = Vector3Int.FloorToInt(BlockFaceHighlighter.SelectedBlockCoords);
                projectManager.DebugText = $"{subGrid},  Face: {BlockFaceHighlighter.SelectedBlockFace}";
            }

        }
    }

    public void PlaceBlock()
    {
        if (projectManager.CurrentProject == null || projectManager.CurrentProject.data == null)
        {
            Debug.LogWarning("No active project to place blocks in");
            return;
        }
        
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayer))
        {
            var hitPoint = hit.point + hit.normal * 0.001f; 
            var snappedPosition = SnapToSubGrid(hitPoint, projectManager.GetGridSize());
            
            if (!IsWithinBounds(snappedPosition, projectManager.CurrentProject.data.size))
            {
                Debug.LogWarning($"Cannot place block outside bounds {snappedPosition}");
                return;
            }
            
            if (BlockExistsAtPosition(snappedPosition))
            {
                Debug.LogWarning($"Block already exists at position {snappedPosition}");
                return;
            }
            
            var newBlock = CreateBlockData(snappedPosition);
            var command = new AddBlockCommand(newBlock);
            projectManager.ExecuteCommand(command);
            
            projectManager.CurrentProject.InitializeFromData(projectManager.CurrentProject.data, blockManager);
        }
    }

    public BlockData GetBlockDataFromRay(RaycastHit hit)
    {
        var hitPoint = hit.point - hit.normal * 0.001f; // Slightly offset inside
        if (hitPoint.y < 0) hitPoint.y = 0;

        var snappedPosition = SnapToSubGrid(hitPoint, projectManager.GetGridSize());
        
        var hitNormal = hit.normal;
        var faceName = DetermineHitFace(hitNormal);
        var blockData = FindBlockAtPosition(snappedPosition);

        return blockData;
    }
    
    public void SelectBlock()
    {
        if (projectManager.CurrentProject == null) return;

        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectionLayer))
        {
            var hitPoint = hit.point - hit.normal * 0.001f; // Slightly offset inside
            if (hitPoint.y < 0) hitPoint.y = 0;

            var snappedPosition = SnapToSubGrid(hitPoint, projectManager.GetGridSize());
            
            // Get the hit normal and determine which face was hit
            var hitNormal = hit.normal;
            var faceName = DetermineHitFace(hitNormal);
            var blockData = FindBlockAtPosition(snappedPosition);

            if (blockData == null)
            {
                Debug.LogWarning($"Cannot select block {snappedPosition}");
                return;
            }

            var blockFace = GetFaceFromName(blockData, faceName);
            
            blockEditorUI.SelectBlock(blockData);
            blockEditorUI.SelectFace(blockFace);
        }
    }
    
    public void RemoveBlock() 
    {
        if (projectManager.CurrentProject == null) return;


        var coords = BlockFaceHighlighter.SelectedBlockCoords;

        var blockToRemove = FindBlockAtPosition(Vector3Int.FloorToInt(coords));
        if (blockToRemove != null)
        {
            var command = new RemoveBlockCommand(blockToRemove);
            projectManager.ExecuteCommand(command);
                
            // Update mesh
            projectManager.CurrentProject.InitializeFromData(projectManager.CurrentProject.data, blockManager);
        }
        else
        {
            Debug.LogWarning("Cannot remove block outside bounds");
        }
    }

    public static Vector3Int SnapToSubGrid(Vector3 position, Vector3Int size)
    {
        var gridSize = new Vector3( 1.0f / size.x, 1.0f / size.y, 1.0f / size.z);
        
        // position += Vector3.one * 0.5f;
        
        return new Vector3Int(
            Mathf.FloorToInt(position.x / gridSize.x),
            Mathf.FloorToInt(position.y / gridSize.y),
            Mathf.FloorToInt(position.z / gridSize.z)
        );
    }
    
    private bool IsWithinBounds(Vector3Int position, Vector3Int size)
    {
        return position.x >= 0 && position.x < size.x &&
               position.y >= 0 && position.y < size.y &&
               position.z >= 0 && position.z < size.z;
    }
    
    private bool BlockExistsAtPosition(Vector3Int position)
    {
        return FindBlockAtPosition(position) != null;
    }
    
    private BlockData FindBlockAtPosition(Vector3Int position)
    {
        if (projectManager.CurrentProject?.data?.blocks == null) return null;
        
        foreach (var block in projectManager.CurrentProject.data.blocks)
        {
            if (block.position == position)
            {
                return block;
            }
        }
        return null;
    }
    
    private BlockData CreateBlockData(Vector3Int position)
    {
        return new BlockData
        {
            position = position,
            top = new BlockFace { texture = defaultTexture, uv = defaultUV },
            bottom = new BlockFace { texture = defaultTexture, uv = defaultUV },
            front = new BlockFace { texture = defaultTexture, uv = defaultUV },
            back = new BlockFace { texture = defaultTexture, uv = defaultUV },
            left = new BlockFace { texture = defaultTexture, uv = defaultUV },
            right = new BlockFace { texture = defaultTexture, uv = defaultUV }
        };
    }

    public static string DetermineHitFace(Vector3 normal)
    {
        normal = normal.normalized;
        
        var threshold = 0.001f;
        
        if (Vector3.Distance(normal, Vector3.up) < threshold) return "top";
        if (Vector3.Distance(normal, Vector3.down) < threshold) return "bottom";
        if (Vector3.Distance(normal, Vector3.forward) < threshold) return "front";
        if (Vector3.Distance(normal, Vector3.back) < threshold) return "back";
        if (Vector3.Distance(normal, Vector3.left) < threshold) return "left";
        return Vector3.Distance(normal, Vector3.right) < threshold ? "right" : "unknown";
    }

    public static BlockFace GetFaceFromName(BlockData block, string faceName)
    {
        return faceName switch
        {
            "top" => block.top,
            "bottom" => block.bottom,
            "front" => block.front,
            "back" => block.back,
            "left" => block.left,
            "right" => block.right,
            _ => null
        };
    }
}
