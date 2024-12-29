using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementAction : MonoBehaviour
{
    [SerializeField] private LayerMask raycastLayer; 
    [SerializeField] private float panSpeed = 10f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 100f; 

    private Vector3 rotationPivot;
    private bool isRotating = false; 
    
    private void Update() {
        if (Mouse.current.middleButton.wasPressedThisFrame) {
            StartRotation();
        }

        if (Mouse.current.middleButton.wasReleasedThisFrame) {
            EndRotation();
        }

        if (isRotating) {
            RotateCamera(Mouse.current.delta.ReadValue());
        }
    }

    private void StartRotation() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastLayer)) {
            rotationPivot = hit.point; // Set the pivot to the hit point
            isRotating = true; // Enable rotation
        } else {
            isRotating = false;
        }
    }

    private void RotateCamera(Vector2 inputDelta) {
        if (!isRotating) return; 

        float horizontalRotation = inputDelta.x * rotateSpeed * Time.deltaTime;
        float verticalRotation = -inputDelta.y * rotateSpeed * Time.deltaTime;

        Camera.main.transform.RotateAround(rotationPivot, Vector3.up, horizontalRotation);
        Camera.main.transform.RotateAround(rotationPivot, Camera.main.transform.right, verticalRotation);
    }

    private void EndRotation() {
        isRotating = false; // Disable rotation
    }
    
    
    public void PanCamera(Vector2 input) {
        var right = Camera.main.transform.right; 
        var up = Vector3.up;                     

        var moveDirection = (right * input.x + up * input.y) * panSpeed * Time.deltaTime;

        Camera.main.transform.Translate(moveDirection, Space.World);
    }
    
    public void ZoomCamera(Vector2 scrollInput) {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastLayer)) {
            var zoomTarget = hit.point; 
            
            var cameraPosition = Camera.main.transform.position;

            var zoomDirection = (zoomTarget - cameraPosition).normalized;

            var zoomAmount = scrollInput.y * zoomSpeed * Time.deltaTime;
            var newCameraPosition = cameraPosition + zoomDirection * zoomAmount;

            var distanceToTarget = Vector3.Distance(newCameraPosition, zoomTarget);
            
            if (distanceToTarget >= minDistance && distanceToTarget <= maxDistance) {
                Camera.main.transform.position = newCameraPosition;
            } else {
                Debug.LogWarning("Zoom limit reached.");
            }
        } else {
            Debug.LogWarning("No valid object hit to zoom into.");
        }
    }
}
