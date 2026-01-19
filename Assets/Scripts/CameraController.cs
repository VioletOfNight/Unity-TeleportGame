using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private bool isRotating = false; // Aynı anda iki kez dönmeyi engellemek için

    public void RotateMap()
    {
        // Eğer zaten dönüyorsa yeni komutu işleme
        if (isRotating) return;

        // Hedef açıyı belirle (Mevcut Y açısı + 90 derece)
        float targetY = transform.eulerAngles.y + 90f;
        StartCoroutine(RotateSmoothly(targetY));
    }

    IEnumerator RotateSmoothly(float targetY)
    {
        isRotating = true;
        float duration = 0.5f; // Dönüş hızı (Saniye)
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, targetY, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Yumuşak bir geçiş (Lerp) ile döndür
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed / duration);
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;
    }
}