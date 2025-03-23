using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorUIManager : MonoBehaviour {
    private static readonly int GridScale = Shader.PropertyToID("_GridScale");
    public UIDocument uiDocument;
    public Material mainGridMaterial;
    
    private ProjectManager projectManager;
    private BlockManager blockManager;
    
    private string selectedTexture;
    private Label debugText;

    [SerializeField] private PlaceBlockAction placeBlockAction;

    public void Initialize(ProjectManager manager, BlockManager blockMng) {
        projectManager = manager;
        blockManager = blockMng;
    }

    private void Start() {
        var root = uiDocument.rootVisualElement;

        var saveButton = root.Q<Button>("saveButton");
        var newButton = root.Q<Button>("newButton");
        var deleteButton = root.Q<Button>("deleteButton");
        var openFolderButton = root.Q<Button>("openFolderButton");
        debugText = root.Q<Label>("debugText");
        
        PreventPanelInput(root.Q<VisualElement>("toolbar"));
        PreventPanelInput(root.Q<VisualElement>("leftPanel"));
        PreventPanelInput(root.Q<VisualElement>("rightPanel"));

        saveButton.clicked += SaveProject;
        newButton.clicked += CreateNewProject;
        deleteButton.clicked += DeleteButtonOnClicked;
        openFolderButton.clicked += OpenFolderButtonOnClicked;
        
        PopulateBlockList();
        PopulateTextureList(); 
    }

    private void Update()
    {
        debugText.text = projectManager.DebugText;
    }

    private void OpenFolderButtonOnClicked()
    {
        var folder = StandaloneFileBrowser.OpenFolderPanel("Open Project Folder", "", false);
        foreach (var entry in folder)
        {
            Debug.Log($"Folder {entry}");
        }
    }

    private void DeleteButtonOnClicked()
    {
        if (projectManager.CurrentProject == null)
        {
            return;
        }
        ShowModal($"Are you sure you want to delete {projectManager.CurrentProject.data.blockName}?", "Delete", () =>
        {
            
            var pathToDelete = projectManager.CurrentProject.data.OwnerFilename;
            File.Delete(pathToDelete);
            PopulateBlockList();
            projectManager.CurrentProject = null;
            
        }, "Cancel", HideModal);
    }

    private void PopulateTextureList()
    {
        var root = uiDocument.rootVisualElement;
        var textureList = root.Q<ScrollView>("textureList");
        textureList.Clear();

        var textures = Resources.LoadAll<Texture2D>("Textures");
        foreach (var texture in textures)
        {
            var textureElement = new Button();
            textureElement.name = $"texture_{texture.name}";
            textureElement.AddToClassList("texture-button");
            
            var image = new Image();
            image.image = texture;
            image.AddToClassList("texture-image");
            
            textureElement.Add(image);
            textureList.Add(textureElement);

            // Add click handler for texture selection
            textureElement.RegisterCallback<ClickEvent>(evt => {
                selectedTexture = texture.name;
                // Update visual selection
                foreach (var element in textureList.Children())
                {
                    if (element == textureElement)
                        element.AddToClassList("selected");
                    else
                        element.RemoveFromClassList("selected");
                }
            });
        }
    }

    private List<string> GetExistingBlockFiles()
    {
        Debug.Log($"Persistent Storage: {Application.persistentDataPath}");
        var files = Directory.GetFiles(Application.persistentDataPath, "*.json");

        foreach (var entry in files)
        {
            Debug.Log($"[Block file] {entry}");
        }

        return files.ToList();
    }
    
    private List<string> GetExistingBlockNames()
    {
        var files = Directory.GetFiles(Application.persistentDataPath, "*.json");

        return files.Select(entry => blockManager.LoadInnerBlock(entry)).Select(standardBlocks => standardBlocks.blockName).ToList();
    }
    
    private void PopulateBlockList() {
        var root = uiDocument.rootVisualElement;
        var blockList = root.Q<ScrollView>("blockList");
        blockList.Clear();
                                             
        var files = GetExistingBlockFiles();
        
        foreach (string file in files) {
            var standardBlocks = blockManager.LoadInnerBlock(file);
            standardBlocks.OwnerFilename = file;
            
            var blockButton = new Button { text = standardBlocks.blockName };
            if (projectManager.CurrentProject?.data?.blockName == standardBlocks.blockName)
            {
                if (!blockButton.ClassListContains("button-selected"))
                    blockButton.AddToClassList("button-selected");
            }
            blockButton.clicked += () => LoadBlockFromFile(file);

            blockList.Add(blockButton);
        }

        Debug.Log("Block list populated.");
    }
    
    private void LoadBlockFromFile(string filePath) {
        if (!File.Exists(filePath)) {
            Debug.LogError($"File not found: {filePath}");
            return;
        }

        projectManager.ClearProject(false);
        projectManager.CurrentProject = blockManager.LoadBlocksFromJson(filePath);
        mainGridMaterial.SetFloat(GridScale, projectManager.CurrentProject.data.size.x);

        PopulateBlockList();
        
        Debug.Log($"Loaded block from {filePath}");
    }

    
    private void PreventPanelInput(VisualElement panel) {
        panel.RegisterCallback<PointerDownEvent>(evt => evt.StopPropagation());
        panel.RegisterCallback<PointerMoveEvent>(evt => evt.StopPropagation());
        panel.RegisterCallback<PointerUpEvent>(evt => evt.StopPropagation());
        panel.RegisterCallback<WheelEvent>(evt => evt.StopPropagation());
    }

    private void CreateNewProject() {
        VisualElement root = uiDocument.rootVisualElement;
        VisualElement dialog = root.Q<VisualElement>("newProjectDialog");
        dialog.style.display = DisplayStyle.Flex;

        DropdownField sizeInput = dialog.Q<DropdownField>("sizeInput");
        var choices = new List<string> { "2x2x2", "3x3x3", "4x4x4", "5x5x5", "6x6x6", "7x7x7", "8x8x8", "9x9x9", "10x10x10", "11x11x11", "12x12x12", "13x13x13", "14x14x14", "15x15x15", "16x16x16" };
        sizeInput.choices = choices;
        sizeInput.value = choices[6];
        
        TextField nameInput = dialog.Q<TextField>("nameInput");
        Button createButton = dialog.Q<Button>("createButton");
        Button cancelButton = dialog.Q<Button>("cancelBtn");
        Label errorLabel = dialog.Q<Label>("errorLabel");

        cancelButton.clicked += () =>
        {
            dialog.style.display = DisplayStyle.None;
        };

        createButton.clicked += () => {
            errorLabel.style.display = DisplayStyle.None;
            var sizeText = sizeInput.value;
            var blockName = nameInput.value;

            if (blockName.Length < 3) {
                ShowModal($"Block name must be at least 3 characters.", "Ok", HideModal);

                return;
            }
            
            var existingBlocks = GetExistingBlockNames();
            var exists = existingBlocks.Any(s => s.Equals(blockName, StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                ShowModal($"Block of that name {blockName} already exists", "Ok", HideModal);

                return;
            }
                                                   
            if (ParseGridSize(sizeText, out Vector3Int size)) {
                projectManager.CreateNewProject(size, nameInput.value);
                mainGridMaterial.SetFloat(GridScale, projectManager.CurrentProject.data.size.x);

                PopulateBlockList();
                
                Debug.Log($"New project created with name: {blockName} and size: {size}");
                
                dialog.style.display = DisplayStyle.None;
                errorLabel.style.display = DisplayStyle.None;
                SaveProject();
            } else {
                errorLabel.text = "Invalid size format. Use NxNxN, e.g., 4x4x4.";
                errorLabel.style.display = DisplayStyle.Flex;
            }
        };
    }

    private void SaveProject() {
        if (projectManager.CurrentProject == null) {
            Debug.LogError("No project to save!");
            return;
        }
            
        blockManager.SaveBlockToJson(projectManager.CurrentProject.data.blockName, projectManager.CurrentProject);
        PopulateBlockList();
    }

    private void LoadProject(string blockName) {
        var path = Path.Combine(Application.persistentDataPath, $"{blockName}.json");

        if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
            
            projectManager.ClearProject(false);
            projectManager.CurrentProject = blockManager.LoadBlocksFromJson(path);
            Debug.Log($"Project loaded from {path}");
        } else {
            Debug.LogError("Invalid or no file selected.");
        }
    }

    private bool ParseGridSize(string sizeText, out Vector3Int size) {
        size = Vector3Int.zero;
        string[] parts = sizeText.Split('x');
        if (parts.Length != 3) return false;

        if (int.TryParse(parts[0], out int x) &&
            int.TryParse(parts[1], out int y) &&
            int.TryParse(parts[2], out int z)) {
            size = new Vector3Int(x, y, z);
            return true;
        }
        return false;
    }
    
    public void ShowModal(string messageText, string button1Text, Action onButton1Clicked = null, string button2Text = null, Action onButton2Clicked = null) {
        var root = uiDocument.rootVisualElement;
        var modal = root.Q<VisualElement>("genericModal");
        var modalText = modal.Q<Label>("modalText");
        var button1 = modal.Q<Button>("modalButton1");
        var button2 = modal.Q<Button>("modalButton2");

        modalText.text = messageText;
        button1.text = button1Text;
        button2.text = button2Text;

        if (string.IsNullOrEmpty(button2.text))
        {
            button2.style.display = DisplayStyle.None;
        }

        void OnButton1Clicked(ClickEvent evt)
        {
            onButton1Clicked?.Invoke();
            HideModal();
        }

        void OnButton2Clicked(ClickEvent evt)
        {
            onButton2Clicked?.Invoke();
            HideModal();
        }
        
        button1.RegisterCallbackOnce<ClickEvent>(OnButton1Clicked);
        button2.RegisterCallbackOnce<ClickEvent>(OnButton2Clicked);

        modal.style.display = DisplayStyle.Flex;
    }
    /// <summary>
    /// Hides the modal dialog.
    /// </summary>
    public void HideModal() {
        var root = uiDocument.rootVisualElement;
        var modal = root.Q<VisualElement>("genericModal");
        var button1 = modal.Q<Button>("modalButton1");
        var button2 = modal.Q<Button>("modalButton2");
        
        modal.style.display = DisplayStyle.None;
    }
}