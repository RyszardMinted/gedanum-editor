using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class BlockEditorUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private PlaceBlockAction placeBlockAction;
    
    private ProjectManager projectManager;
    private BlockManager blockManager;
    
    private string selectedTexture = "default";
    private BlockFace selectedFace;
    private BlockData selectedBlock;
    private string selectedFaceName;
    
    public void Initialize(ProjectManager manager, BlockManager blockMng)
    {
        projectManager = manager;
        blockManager = blockMng;
        
        SetupUI();
    }
    
    private void SetupUI()
    {
        var root = uiDocument.rootVisualElement;
        var textureList = root.Q<ScrollView>("textureList");
        
        // Subscribe to texture selection
        textureList.RegisterCallback<ClickEvent>(evt => {
            var clickedElement = evt.target as VisualElement;
            if (clickedElement != null && clickedElement.name.StartsWith("texture_"))
            {
                selectedTexture = clickedElement.name.Replace("texture_", "");
                UpdateSelectedBlockFace();
            }
        });
    }
    
    public void SelectBlock(BlockData block)
    {
        selectedBlock = block;
        Debug.Log($"Selected block: {block.position}");
    }
    
    public void SelectFace(BlockFace face)
    {
        selectedFace = face;
        selectedFaceName = face.texture;
    }
    
    private void UpdateSelectedBlockFace()
    {                                                                                
        if (selectedBlock == null || selectedFace == null || string.IsNullOrEmpty(selectedTexture)) return;
        
        var command = new UpdateBlockFaceCommand(selectedBlock, selectedFace, selectedTexture);
        projectManager.ExecuteCommand(command);
        
        // Update the mesh
        projectManager.CurrentProject.InitializeFromData(projectManager.CurrentProject.data);
    }
} 