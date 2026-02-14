using UnityEngine;

public class AbsoluteMouseRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float maxYaw = 10f;      // Max horizontal rotation in degrees
    public float maxPitch = 5f;     // Max vertical rotation in degrees

    void Update()
    {
        // Get mouse position normalized to -1 to 1
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Clamp values just in case
        mouseX = Mathf.Clamp(mouseX, -1f, 1f);
        mouseY = Mathf.Clamp(mouseY, -1f, 1f);

        // Calculate rotation angles
        float yaw = mouseX * maxYaw;           // left-right
        float pitch = -mouseY * maxPitch;      // up-down (invert Y for natural feel)

        // Apply rotation directly
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
