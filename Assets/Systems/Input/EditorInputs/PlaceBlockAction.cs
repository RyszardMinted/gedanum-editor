using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceBlockAction : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private LayerMask placementLayer;
    [SerializeField] private BlockEditorUIManager blockEditorUI;

    [SerializeField] private GameObject cursor;
    
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
            cursor.transform.position = hit.point;
            var subGrid = SnapToSubGrid(hit.point);
            projectManager.DebugText = subGrid.ToString();
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
            cursor.transform.position = hit.point;
            
            var hitPoint = hit.point + hit.normal * 0.001f; // Slightly offset to avoid z-fighting
            var snappedPosition = SnapToSubGrid(hitPoint);
            
            // Validate position is within bounds
            if (!IsWithinBounds(snappedPosition, projectManager.CurrentProject.data.size))
            {
                Debug.LogWarning("Cannot place block outside bounds");
                return;
            }
            
            // Check if block already exists at position
            if (BlockExistsAtPosition(snappedPosition))
            {
                Debug.LogWarning("Block already exists at position");
                return;
            }
            
            // Create new block data
            var newBlock = CreateBlockData(snappedPosition);
            
            // Add block using command pattern
            var command = new AddBlockCommand(newBlock);
            projectManager.ExecuteCommand(command);
            
            // Update mesh
            projectManager.CurrentProject.InitializeFromData(projectManager.CurrentProject.data);
        }
    }
    
    public void SelectBlock()
    {
        if (projectManager.CurrentProject == null) return;

        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var hitPoint = hit.point - hit.normal * 0.001f; // Slightly offset inside
            if (hitPoint.y < 0) hitPoint.y = 0;

            var snappedPosition = SnapToSubGrid(hitPoint);
            
            var selectedBlock = FindBlockAtPosition(snappedPosition);
            if (selectedBlock != null)
            {
                blockEditorUI.SelectBlock(selectedBlock);
            }
        }
    }
    
    public void RemoveBlock() 
    {
        if (projectManager.CurrentProject == null) return;

        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit)) return;
        
        var hitPoint = hit.point + hit.normal * 0.001f; // Slightly offset inside
        if (hitPoint.y < 0) hitPoint.y = 0;
        var snappedPosition = SnapToSubGrid(hitPoint);
            
        var blockToRemove = FindBlockAtPosition(snappedPosition);
        if (blockToRemove != null)
        {
            var command = new RemoveBlockCommand(blockToRemove);
            projectManager.ExecuteCommand(command);
                
            // Update mesh
            projectManager.CurrentProject.InitializeFromData(projectManager.CurrentProject.data);
        }
        else
        {
            Debug.LogWarning("Cannot remove block outside bounds");
        }
    }

    private Vector3Int SnapToSubGrid(Vector3 position)
    {
        if (projectManager.CurrentProject == null) return Vector3Int.zero;

        var gridSize = new Vector3( 1.0f / projectManager.CurrentProject.data.size.x, 1.0f / projectManager.CurrentProject.data.size.y, 1.0f / projectManager.CurrentProject.data.size.z);
        
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
}
