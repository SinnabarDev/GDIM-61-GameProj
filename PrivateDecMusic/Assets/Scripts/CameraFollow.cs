using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
                if(Mathf.Abs(desiredPosition.x) > 1.5f )
        {
            if (desiredPosition.x > 0)
            {
                desiredPosition.x = 1.5f;
            }
            else if (desiredPosition.x < 0)
            {
                desiredPosition.x = -1.5f;
            }
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}