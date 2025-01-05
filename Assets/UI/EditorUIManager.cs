using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorUIManager : MonoBehaviour {
    public UIDocument uiDocument;

    private ProjectManager projectManager;
    private BlockManager blockManager;

    public void Initialize(ProjectManager manager, BlockManager blockMng) {
        projectManager = manager;
        blockManager = blockMng;
    }

    private void Start() {
        var root = uiDocument.rootVisualElement;

        Button saveButton = root.Q<Button>("saveButton");
        Button newButton = root.Q<Button>("newButton");

        PreventPanelInput(root.Q<VisualElement>("toolbar"));
        PreventPanelInput(root.Q<VisualElement>("leftPanel"));
        PreventPanelInput(root.Q<VisualElement>("rightPanel"));

        saveButton.clicked += SaveProject;
        // loadButton.clicked += LoadProject;
        
        newButton.clicked += CreateNewProject;
        
        PopulateBlockList();
    }
    
    private void PopulateBlockList() {
        var root = uiDocument.rootVisualElement;
        var blockList = root.Q<ScrollView>("blockList");
        blockList.Clear();
                                             
        Debug.Log($"Persistent Storage: {Application.persistentDataPath}");
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");
        foreach (string file in files) {
            var standardBlocks = blockManager.LoadInnerBlock(file);
            

            Button blockButton = new Button { text = standardBlocks.blockName };
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

        TextField sizeInput = dialog.Q<TextField>("sizeInput");
        TextField nameInput = dialog.Q<TextField>("nameInput");
        Button createButton = dialog.Q<Button>("createButton");
        Label errorLabel = dialog.Q<Label>("errorLabel");

        createButton.clicked += () => {
            string sizeText = sizeInput.value;
            string blockName = nameInput.value;

            if (blockName.Length < 3) {
                errorLabel.text = "Block name must be at least 3 characters.";
                errorLabel.style.display = DisplayStyle.Flex;
                return;
            }
                                                   
            if (ParseGridSize(sizeText, out Vector3Int size)) {
                projectManager.CreateNewProject(size, nameInput.value);
                PopulateBlockList();
                
                Debug.Log($"New project created with name: {blockName} and size: {size}");
                
                dialog.style.display = DisplayStyle.None;
                errorLabel.style.display = DisplayStyle.None;
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
}