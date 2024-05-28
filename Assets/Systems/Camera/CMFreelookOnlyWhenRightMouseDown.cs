using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CMFreelookOnlyWhenRightMouseDown : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;
    public float zoomSpeed = 2f; // Speed of zoom
    public float minRadius = 2f; // Minimum zoom distance
    public float maxRadius = 20f; // Maximum zoom distance

    void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }

    private void Update()
    {
        HandleCameraZoom();
    }
    public float GetAxisCustom(string axisName)
    {
        if (axisName == "Mouse X")
        {
            if (Input.GetMouseButton(1))
            {
                return Input.GetAxis("Mouse X");
            }
            else
            {
                return 0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (Input.GetMouseButton(1))
            {
                return Input.GetAxis("Mouse Y");
            }
            else
            {
                return 0;
            }
        }
        return UnityEngine.Input.GetAxis(axisName);
    }

    private void HandleCameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            // Adjust the radius of the orbits
            freeLookCamera.m_Orbits[0].m_Radius += scroll * zoomSpeed;
            freeLookCamera.m_Orbits[1].m_Radius += scroll * zoomSpeed;
            freeLookCamera.m_Orbits[2].m_Radius += scroll * zoomSpeed;

            // Clamp the radius to avoid extreme zoom in/out
            freeLookCamera.m_Orbits[0].m_Radius = Mathf.Clamp(freeLookCamera.m_Orbits[0].m_Radius, minRadius, maxRadius);
            freeLookCamera.m_Orbits[1].m_Radius = Mathf.Clamp(freeLookCamera.m_Orbits[1].m_Radius, minRadius, maxRadius);
            freeLookCamera.m_Orbits[2].m_Radius = Mathf.Clamp(freeLookCamera.m_Orbits[2].m_Radius, minRadius, maxRadius);
        }
    }
}