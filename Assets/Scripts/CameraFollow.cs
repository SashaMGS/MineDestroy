using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Игрок (объект, за которым следует камера)
    public float smoothSpeed = 0.025f; // Скорость сглаживания
    public float smoothSpeedMax = 0.5f; // Скорость сглаживания при большом отдалении
    public float maxDistanceNormal = 10f;
    public float minDistanceNormal = 8.5f;
    public float distance;
    public Vector3 offset = new Vector3(0f, 7.5f, -2.5f); // Смещение камеры относительно игрока

    private float smoothSpeedStart = 0.025f; // Обычная скорость сглаживания 

    private void Start()
    {
        smoothSpeedStart = smoothSpeed;
    }

    void LateUpdate()
    {
        if (player == null) return; // Проверка, есть ли игрок

        distance = Vector3.Distance(transform.position, player.position);

        if (Vector3.Distance(transform.position, player.position) > maxDistanceNormal ||
            Vector3.Distance(transform.position, player.position) < minDistanceNormal)
            smoothSpeed = smoothSpeedMax;
        else
            smoothSpeed = smoothSpeedStart;

        // Целевое положение камеры с учетом смещения
        Vector3 targetPosition = player.position + offset;

        // Плавное перемещение камеры
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
