using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public float bobbingSpeed = 0.18f; // Частота качания
    public float bobbingAmount = 0.2f; // Максимальное смещение
    public bool isRun;

    private PlController plController;
    private float timer = 0.0f;
    private Vector3 originalPosition;

    void Start()
    {
        plController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlController>();
        originalPosition = transform.localPosition;
    }

    void FixedUpdate()
    {
        isRun = plController.isRun;
        // Перемещение камеры по оси Y с использованием синусоиды
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            if (isRun)
                timer += bobbingSpeed * 2;
            else
                timer += bobbingSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }
        }

        Vector3 newPosition = transform.localPosition;
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange *= totalAxes;
            newPosition.y = originalPosition.y + translateChange;
        }
        else
        {
            newPosition.y = originalPosition.y;
        }

        transform.localPosition = newPosition;
    }
}
