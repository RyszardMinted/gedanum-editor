using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private EditorUIManager editorUIManager;
    [SerializeField] private BlockManager blockManager;
    [SerializeField] private PlaceBlockAction blockAction;
    [SerializeField] private BlockEditorUIManager blockEditorUIManager;

    private void Awake() {
        var projectManager = new ProjectManager(blockManager);
        editorUIManager.Initialize(projectManager, blockManager);          
        blockAction.Initialize(projectManager, blockManager);      
        blockEditorUIManager.Initialize(projectManager, blockManager);

        Debug.Log("Bootstrapper: Initialization complete.");
    }
}
