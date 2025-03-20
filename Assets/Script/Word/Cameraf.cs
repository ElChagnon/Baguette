using UnityEngine;

public class Cameraf : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player; // Référence au joueur
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Décalage par rapport au joueur
    public float rotationSpeed = 10f; // Vitesse de rotation de la caméra
    public float followSpeed = 2f; // Vitesse de suivi
    public float mouseSensitivity = 1f; // Sensibilité de la souris

    [Header("Camera Restrictions")]
    private bool isRestricted = false; // La caméra est-elle bloquée ?
    private Vector3 restrictedPosition; // Position fixe de la caméra si bloquée

    private float pitch = 0f; // Angle vertical de la caméra
    private float yaw = 0f; // Angle horizontal de la caméra

    void Update()
    {
        if (player == null) return;

        HandleCameraRotation();
        HandleCameraPosition();
    }

    private void HandleCameraRotation()
    {
        // Rotation de la caméra avec la souris
        if (Input.GetMouseButton(1)) // Clic droit de la souris
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, -90f, 90f); // Limiter l'angle vertical
        }

        // Rotation de la caméra avec le joystick droit
        if (Mathf.Abs(Input.GetAxis("RightJoystickHorizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("RightJoystickVertical")) > 0.1f)
        {
            yaw += Input.GetAxis("RightJoystickHorizontal") * rotationSpeed;
            pitch -= Input.GetAxis("RightJoystickVertical") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, -90f, 90f); // Limiter l'angle vertical
        }

        // Appliquer la rotation de la caméra
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.LookAt(player); // Toujours regarder le joueur
    }

    private void HandleCameraPosition()
    {
        Vector3 targetPosition;

        if (isRestricted)
        {
            // Garder la caméra fixe sur la position restreinte
            targetPosition = restrictedPosition;
        }
        else
        {
            // Suivre le joueur avec un décalage
            targetPosition = player.position + offset;
        }

        // Déplacer la caméra de manière fluide
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CameraSection"))
        {
            isRestricted = true;
            restrictedPosition = other.transform.position + offset;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CameraSection"))
        {
            isRestricted = false;
        }
    }
}