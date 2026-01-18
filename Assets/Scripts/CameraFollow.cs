using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;              // Asigna el Player aquí
    public float smoothTime = 0.3f;       // Tiempo de suavizado (más alto = más lento)
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Distancia de la cámara

    private Vector3 velocity = Vector3.zero; // Variable interna para SmoothDamp

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No hay target asignado.");
            return;
        }

        // 1. Calcula la posición objetivo FUTURA (para que la cámara "adelante" al movimiento)
        Vector3 targetPosition = target.position + offset;

        // 2. Usa SmoothDamp para un seguimiento más natural (especialmente en saltos)
        //    Solo suaviza en X e Y, Z se mantiene fija en el offset
        Vector3 desiredPosition = new Vector3(
            targetPosition.x,
            targetPosition.y,
            transform.position.z // Mantenemos la Z actual de la cámara
        );

        // 3. Aplica el suavizado
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );
    }
}