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
    
    public void Initialize(ProjectManager manager, BlockManager blockMng)
    {
        projectManager = manager;
        blockManager = blockMng;
        
        SetupUI();
    }
    
    private void SetupUI()
    {
        var root = uiDocument.rootVisualElement;
        
        // Setup texture selection panel
        var texturePanel = root.Q<VisualElement>("texturePanel");
        var textureList = root.Q<ListView>("textureList");
        
        // Get available textures from Resources folder
        var textures = Resources.LoadAll<Texture2D>("Textures").Select(t => t.name).ToList();
        textureList.itemsSource = textures;
        textureList.onSelectionChange += objects => {
            if (objects.FirstOrDefault() is string textureName)
            {
                selectedTexture = textureName;
                UpdateSelectedBlockFace();
            }
        };
        
        // Setup face selection buttons
        SetupFaceButton(root, "topFaceBtn", "top");
        SetupFaceButton(root, "bottomFaceBtn", "bottom");
        SetupFaceButton(root, "frontFaceBtn", "front");
        SetupFaceButton(root, "backFaceBtn", "back");
        SetupFaceButton(root, "leftFaceBtn", "left");
        SetupFaceButton(root, "rightFaceBtn", "right");
    }
    
    private void SetupFaceButton(VisualElement root, string buttonName, string faceName)
    {
        var button = root.Q<Button>(buttonName);
        if (button != null)
        {
            button.clicked += () => SelectFace(faceName);
        }
    }
    
    public void SelectBlock(BlockData block)
    {
        selectedBlock = block;
        if (block != null)
        {
            // Update UI to show current block's textures
            UpdateTextureUI();
        }
    }
    
    private void SelectFace(string faceName)
    {
        if (selectedBlock == null) return;
        
        selectedFace = faceName switch
        {
            "top" => selectedBlock.top,
            "bottom" => selectedBlock.bottom,
            "front" => selectedBlock.front,
            "back" => selectedBlock.back,
            "left" => selectedBlock.left,
            "right" => selectedBlock.right,
            _ => null
        };
        
        if (selectedFace != null)
        {
            // Update UI to show selected face's texture
            var textureList = uiDocument.rootVisualElement.Q<ListView>("textureList");
            var textureIndex = textureList.itemsSource.Cast<string>().ToList().IndexOf(selectedFace.texture);
            if (textureIndex >= 0)
            {
                textureList.selectedIndex = textureIndex;
            }
        }
    }
    
    private void UpdateSelectedBlockFace()
    {
        if (selectedBlock == null || selectedFace == null || string.IsNullOrEmpty(selectedTexture)) return;
        
        var command = new UpdateBlockFaceCommand(selectedBlock, selectedFace, selectedTexture);
        projectManager.ExecuteCommand(command);
        
        // Update the mesh
        projectManager.CurrentProject.InitializeFromData(projectManager.CurrentProject.data);
    }
    
    private void UpdateTextureUI()
    {
        if (selectedBlock == null) return;
        
        var textureList = uiDocument.rootVisualElement.Q<ListView>("textureList");
        if (selectedFace != null)
        {
            var textureIndex = textureList.itemsSource.Cast<string>().ToList().IndexOf(selectedFace.texture);
            if (textureIndex >= 0)
            {
                textureList.selectedIndex = textureIndex;
            }
        }
    }
} 