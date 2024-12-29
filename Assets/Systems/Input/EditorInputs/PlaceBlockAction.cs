using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceBlockAction : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private LayerMask placementLayer;

    private VoxelEditorInput input;

    public void SetInput(VoxelEditorInput inputToSet)
    {
        input = inputToSet;
    }
    
    public void PlaceBlock() {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayer)) {
            var hitPoint = hit.point;
            var snappedPosition = SnapToGrid(hitPoint);

            Instantiate(blockPrefab, snappedPosition, Quaternion.identity);
        }
    }
    
    public void RemoveBlock() {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            var blockInstance = hit.collider.GetComponent<BlockInstance>();

            if (blockInstance == null) return;
            
            var block = hit.collider.gameObject;
            Destroy(block);
        }
    }

    private Vector3 SnapToGrid(Vector3 position) {
        var gridSize = 1.0f; 
        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            Mathf.Round(position.z / gridSize) * gridSize
        );
    }
}
