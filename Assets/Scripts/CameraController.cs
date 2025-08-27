using UnityEngine;

namespace PigFarm
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float sprintMultiplier = 2f;
        
        [Header("Look Settings")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 90f;
        
        private float verticalRotation = 0f;
        
        void Start()
        {
            // Lock cursor to center of screen
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        void Update()
        {
            HandleMovement();
            HandleRotation();
        }
        
        private void HandleMovement()
        {
            // Get input axes
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            // Calculate movement direction relative to camera orientation
            Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
            moveDirection.Normalize();
            
            // Apply sprint multiplier if shift is held
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * sprintMultiplier : moveSpeed;
            
            // Move the camera
            transform.position += moveDirection * currentSpeed * Time.deltaTime;
        }
        
        private void HandleRotation()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            
            // Horizontal rotation (Y axis)
            transform.Rotate(Vector3.up * mouseX);
            
            // Vertical rotation with clamping
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
            transform.localEulerAngles = new Vector3(verticalRotation, transform.localEulerAngles.y, 0f);
        }
        
        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}