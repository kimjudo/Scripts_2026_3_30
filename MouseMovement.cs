using UnityEngine;
using UnityEngine.InputSystem; 

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    [SerializeField] private Transform cameraPivot;
    float xRotation = 0f;
    float yRotation = 0f;

    private Vector2 lookInput;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        yRotation += mouseX;
        transform.localRotation = Quaternion.AngleAxis(yRotation, Vector3.up);
        cameraPivot.localRotation = Quaternion.AngleAxis(xRotation, Vector3.right);

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}