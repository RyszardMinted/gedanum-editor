using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private EditorUIManager editorUIManager;
    [SerializeField] private BlockManager blockManager;
    [SerializeField] private PlaceBlockAction blockAction;
    
    private void Start() {
        var projectManager = new ProjectManager(blockManager);
        editorUIManager.Initialize(projectManager, blockManager);          
        blockAction.Initialize(projectManager, blockManager);          

        Debug.Log("Bootstrapper: Initialization complete.");
    }
}
