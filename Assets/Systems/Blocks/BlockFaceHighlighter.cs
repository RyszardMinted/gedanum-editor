using UnityEngine;

public class BlockFaceHighlighter : MonoBehaviour
{
    private static readonly int HighlightedFace = Shader.PropertyToID("_HighlightedFace");
    private static readonly int SelectedBlock = Shader.PropertyToID("_SelectedBlock");

    [SerializeField] private LayerMask placementLayer;
    [SerializeField] private PlaceBlockAction placeBlockAction;
    
    private Camera mainCamera;
    private Material blockMaterial;
    private int highlightedFace = -1;
    private BlockData selectedBlock;
    private GameObject previousBlock;
    
    public static Vector3 SelectedBlockCoords;
    public static string SelectedBlockFace;

    private static readonly string[] FaceNames = { "Top", "Bottom", "Front", "Back", "Left", "Right" };

    public BlockData GetSelectedBlock()
    {
        return selectedBlock;
    }
    
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void ReleaseSelection(bool forced = false)
    {
        if (!forced)
        {
            if (highlightedFace == -1) return;
        }

        highlightedFace = -1;
            
        if (blockMaterial != null)
        {
            blockMaterial.SetInt(HighlightedFace, -1);
            blockMaterial.SetVector(SelectedBlock, Vector4.zero);
        }

        if (previousBlock != null)
        {
            var prevMaterial = previousBlock.GetComponent<MeshRenderer>().material;
            if (prevMaterial != null)
            {
                prevMaterial.SetInt(HighlightedFace, -1);
                prevMaterial.SetVector(SelectedBlock, Vector4.zero);
            }
            previousBlock = null;
        }
    }

    private void ClearSelection()
    {
        ReleaseSelection(true);
        // SelectedBlockFace = null;
    }
                
    private void Update()          
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, placementLayer)) 
        {
            var blockInstance = hit.collider.gameObject.GetComponent<BlockInstance>();
            if (blockInstance == null)
            {
                ClearSelection();
                return;
            }
            
            var mesh = blockInstance.gameObject.GetComponent<MeshFilter>().mesh;
            if (mesh == null)
            {
                ClearSelection();
                return;
            }
            
            blockMaterial = blockInstance.gameObject.GetComponent<MeshRenderer>().material;
            var triangleIndex = hit.triangleIndex;
            var faceIndex = GetFaceIndexFromTriangle(mesh, triangleIndex);
            
            var selectedBlockNow = placeBlockAction.GetBlockDataFromRay(hit);
            if (selectedBlockNow != selectedBlock)
            {
                ClearSelection();
            }

            selectedBlock = selectedBlockNow;
            previousBlock = blockInstance.gameObject;

            // Update face name regardless of block data
            if (faceIndex != highlightedFace)
            {
                highlightedFace = faceIndex;
                SelectedBlockFace = FaceNames[faceIndex];
            }

            // Only update block coordinates and material if we have valid block data
            if (selectedBlock != null)
            {
                SelectedBlockCoords = selectedBlock.position;
                if (blockMaterial != null)
                {
                    blockMaterial.SetVector(SelectedBlock, SelectedBlockCoords);
                    blockMaterial.SetInt(HighlightedFace, highlightedFace);
                }
            }
            else
            {
                ClearSelection();
            }
        }
        else
        {
            selectedBlock = null;
            ClearSelection();
        }
    }

    private int GetFaceIndexFromTriangle(Mesh mesh, int triangleIndex)
    {
        var faceIndex = triangleIndex / 2;
        
        Color[] colors = mesh.colors;
        var vertexIndex = faceIndex * 4;
        return Mathf.RoundToInt(colors[vertexIndex].r * 255);
    }
} 