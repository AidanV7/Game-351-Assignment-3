using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // Player character
    public Camera firstPersonCam;     // First-Person Camera
    public Camera orbitCam;           // Orbit Camera
    public Camera mainCam;            // Still camera (static)
    public Vector3 orbitOffset = new Vector3(0, 2, -5); // Default orbit offset
    public float orbitSensitivity = 3f;  // Mouse sensitivity

    private bool isFirstPerson = true;   // Start in FP mode
    private bool useMainCam = false;     // Is MainCam active
    private Vector2 orbitAngles;         // X = yaw, Y = pitch
    private Vector3 defaultOrbitOffset; 

    void Start()
    {
        defaultOrbitOffset = orbitOffset;
        UpdateCameraStates();
    }

    void Update()
    {
        HandleToggle();
        HandleOrbitRotation();
        HandleResetOrbit();
        UpdateCameraPosition();
    }

    // Toggle FP / Orbit camera and MainCam
    void HandleToggle()
    {
        // Toggle FP / Orbit with T
        if (Input.GetKeyDown(KeyCode.T) && !useMainCam)
        {
            isFirstPerson = !isFirstPerson;
            UpdateCameraStates();
        }

        // Toggle MainCam with M
        if (Input.GetKeyDown(KeyCode.M))
        {
            useMainCam = !useMainCam;
            UpdateCameraStates();
        }
    }

    // Reset orbit camera to default over-the-shoulder view
    void HandleResetOrbit()
    {
        if (!isFirstPerson && !useMainCam && Input.GetKeyDown(KeyCode.R))
        {
            orbitOffset = defaultOrbitOffset;
            orbitAngles = Vector2.zero;
        }
    }

    // Rotate orbit camera on mouse drag
    void HandleOrbitRotation()
    {
        if (!isFirstPerson && !useMainCam && Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X") * orbitSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * orbitSensitivity;

            orbitAngles.x += mouseX;
            orbitAngles.y -= mouseY;
            orbitAngles.y = Mathf.Clamp(orbitAngles.y, -40f, 80f);
        }
    }

    // Update camera positions and rotations
    void UpdateCameraPosition()
    {
        if (useMainCam)
        {
            // MainCam stays static
            mainCam.enabled = true;
            firstPersonCam.enabled = false;
            orbitCam.enabled = false;
            return;
        }

        mainCam.enabled = false;

        if (isFirstPerson)
        {
            firstPersonCam.enabled = true;
            orbitCam.enabled = false;
            firstPersonCam.transform.position = player.position + Vector3.up * 1.6f;
            firstPersonCam.transform.rotation = player.rotation;
        }
        else
        {
            firstPersonCam.enabled = false;
            orbitCam.enabled = true;
            Quaternion rotation = Quaternion.Euler(orbitAngles.y, orbitAngles.x, 0);
            Vector3 desiredPos = player.position + rotation * orbitOffset;
            orbitCam.transform.position = desiredPos;
            orbitCam.transform.LookAt(player.position + Vector3.up * 1.5f);
        }
    }

    // Enable/disable cameras based on current mode
    void UpdateCameraStates()
    {
        if (useMainCam)
        {
            mainCam.enabled = true;
            firstPersonCam.enabled = false;
            orbitCam.enabled = false;
        }
        else
        {
            mainCam.enabled = false;
            firstPersonCam.enabled = isFirstPerson;
            orbitCam.enabled = !isFirstPerson;
        }
    }
}