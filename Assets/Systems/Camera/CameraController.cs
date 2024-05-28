using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float borderThickness = 10f; // Distance from screen edge to start moving the camera
    public float cameraSpeed = 10f; // Speed of the camera movement
    public Bounds cameraBounds; // Define the camera bounds for the current area

    private CinemachineVirtualCamera activeVirtualCamera;
    private Transform activeCameraTransform;

    void Start()
    {
        SetActiveVirtualCamera();
    }

    void Update()
    {
        if (activeVirtualCamera != null)
        {
            Vector3 cameraMovement = Vector3.zero;

            if (Input.mousePosition.x <= borderThickness)
            {
                cameraMovement.x -= cameraSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.x >= Screen.width - borderThickness)
            {
                cameraMovement.x += cameraSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.y <= borderThickness)
            {
                cameraMovement.z -= cameraSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.y >= Screen.height - borderThickness)
            {
                cameraMovement.z += cameraSpeed * Time.deltaTime;
            }

            // Apply the camera movement
            activeCameraTransform.Translate(cameraMovement, Space.World);

            // Clamp the camera position within the defined bounds
            Vector3 clampedPosition = activeCameraTransform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, cameraBounds.min.x, cameraBounds.max.x);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, cameraBounds.min.z, cameraBounds.max.z);

            activeCameraTransform.position = clampedPosition;
        }
    }

    public void SetActiveVirtualCamera()
    {
        CinemachineVirtualCamera[] virtualCameras = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var vcam in virtualCameras)
        {
            if (vcam.Priority == 10) // Assuming 10 is the highest priority for the active camera
            {
                activeVirtualCamera = vcam;
                activeCameraTransform = vcam.transform;
                break;
            }
        }
    }

    public void SwitchVirtualCamera(CinemachineVirtualCamera newCamera, Bounds newBounds)
    {
        if (activeVirtualCamera != null)
        {
            activeVirtualCamera.Priority = 0; // Lower the priority of the current camera
        }
        newCamera.Priority = 10; // Set the new camera as the active one
        cameraBounds = newBounds; // Update the camera bounds for the new area
        SetActiveVirtualCamera(); // Update the active camera
    }
}