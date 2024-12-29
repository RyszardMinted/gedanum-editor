using UnityEngine;
using UnityEngine.InputSystem;

public class VoxelEditorController : MonoBehaviour {
    private VoxelEditorInput inputActions; // Your custom input actions class
    [SerializeField] private float flySpeed = 10f; // Movement speed
    [SerializeField] private float lookSpeed = 2f;   // Look sensitivity

    [SerializeField] private PlaceBlockAction placeAction;
    [SerializeField] private CameraMovementAction cameraAction;
    
    
    private Vector2 moveInput = Vector2.zero;        // Input for movement
    private Vector2 lookInput = Vector2.zero;        // Input for mouse look
    private bool isModifierActive = false;  
    
    private void Awake() {
        // Initialize the input actions
        inputActions = new VoxelEditorInput();

        inputActions.Editor.CameraPan.performed += context => CameraPan(context.ReadValue<Vector2>());
        inputActions.Editor.CameraZoom.performed += ctx => CameraZoom(ctx.ReadValue<Vector2>());
            
        inputActions.Editor.PlaceBlock.performed += ctx => PlaceBlock();
        inputActions.Editor.RemoveBlock.performed += ctx => RemoveBlock();

        inputActions.FlyControls.FlyLook.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.FlyControls.FlyLook.canceled += ctx => lookInput = Vector2.zero;
        inputActions.FlyControls.FlyMove.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.FlyControls.FlyMove.canceled += ctx => moveInput = Vector2.zero;
        inputActions.FlyControls.FlyModifier.performed += ctx => EnableFlyMode();
        inputActions.FlyControls.FlyModifier.canceled += ctx => DisableFlyMode();
        
        placeAction.SetInput(inputActions);
    }
    
    private void Update() {
        if (!isModifierActive) return; // Only allow controls when modifier is active

        HandleFlyMovement();
        HandleMouseLook();
    }
    
    private void EnableFlyMode() {
        isModifierActive = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void DisableFlyMode() {
        isModifierActive = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HandleFlyMovement() {
        if (moveInput == Vector2.zero) return;

        // Get the camera's forward, right, and up vectors
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        Vector3 up = Camera.main.transform.up;

        // Calculate movement based on camera orientation
        Vector3 movement = (forward * moveInput.y + right * moveInput.x) * flySpeed * Time.deltaTime;

        // Apply movement
        Camera.main.transform.position += movement;
    }

    private void HandleMouseLook() {
        if (lookInput == Vector2.zero) return;

        // Calculate rotation
        float horizontalRotation = lookInput.x * lookSpeed * Time.deltaTime;
        float verticalRotation = -lookInput.y * lookSpeed * Time.deltaTime;

        // Rotate camera
        Camera.main.transform.Rotate(Vector3.up, horizontalRotation, Space.World);
        Camera.main.transform.Rotate(Vector3.right, verticalRotation, Space.Self);
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