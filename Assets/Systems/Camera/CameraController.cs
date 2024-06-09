using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]
    public float panSpeed = 20f;
    public float panBorderThickness = 10f; // Thickness in pixels
    public Vector2 panLimit; // Limits for panning

    [Header("Camera Zoom")]
    public float scrollSpeed = 20f;
    public float minY = 20f;
    public float maxY = 120f;

    [Header("Camera Rotation")]
    public float rotationSpeed = 100f;
    private Vector3 rotateOrigin;
    private bool isRotating = false;

    private bool canMove = true;

    private void Update()
    {
        if (canMove)
        {
            HandleMovement();
            HandleZoom();
            HandleRotation();
        }

    }

    public void CanMoveCamera()
    {
        canMove = true;
    }

    public void DontMoveCamera()
    {
        canMove = false;
    }

    private void HandleMovement()
    {
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos += forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos -= forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos -= right * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos += right * panSpeed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

        transform.position = pos;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;

        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
            rotateOrigin = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            Vector3 direction = rotateOrigin - Input.mousePosition;
            rotateOrigin = Input.mousePosition;

            transform.Rotate(Vector3.up, -direction.x * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
