using UnityEngine;
using UnityEngine.InputSystem;

public class VoxelEditorController : MonoBehaviour {
    private VoxelEditorInput inputActions; // Your custom input actions class

    [SerializeField] private PlaceBlockAction placeAction;
    [SerializeField] private CameraMovementAction cameraAction;

    private void Awake() {
        // Initialize the input actions
        inputActions = new VoxelEditorInput();

        inputActions.Editor.CameraPan.performed += context => CameraPan(context.ReadValue<Vector2>());
        inputActions.Editor.CameraZoom.performed += ctx => CameraZoom(ctx.ReadValue<Vector2>());
            
        inputActions.Editor.PlaceBlock.performed += ctx => PlaceBlock();
        inputActions.Editor.RemoveBlock.performed += ctx => RemoveBlock();

        // inputActions.Editor.CameraRotate.started += ctx => cameraAction?.StartRotation(ctx); 
        // inputActions.Editor.CameraRotate.performed += ctx =>  RotateCamera(ctx.ReadValue<Vector2>()); // Rotate the camera
        // inputActions.Editor.CameraRotate.canceled += ctx => cameraAction?.EndRotation(ctx); 
    }

    private void CameraZoom(Vector2 scrollDelta)
    {
        cameraAction.ZoomCamera(scrollDelta);
    }
    
    private void OnEnable() {
        inputActions.Enable();
    }

    private void OnDisable() {
        inputActions.Disable();
    }

    private void CameraPan(Vector2 inputDelta)
    {
        cameraAction.PanCamera(inputDelta);
    }

    private void PlaceBlock() {
        placeAction.PlaceBlock();
    }

    private void RemoveBlock() {
        placeAction.RemoveBlock();
    }

    private void RotateCamera(Vector2 inputDelta) {
      //  cameraAction.RotateCamera(inputDelta);
    }
}