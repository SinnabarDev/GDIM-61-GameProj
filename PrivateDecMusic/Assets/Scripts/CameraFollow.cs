using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Header("X Constraints")]
    [SerializeField] private float leftLimit = -100f;
    [SerializeField] private float rightLimit = 100f;
    void LateUpdate()
    {
        if (player == null) return;

        // Desired follow position
        Vector3 desiredPosition = player.position + offset;

        // Clamp only X movement
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, leftLimit, rightLimit);

        // Smooth follow
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed
        );

        // Keep camera Z unchanged
        transform.position = new Vector3(
            smoothedPosition.x,
            smoothedPosition.y,
            transform.position.z
        );
    }
}