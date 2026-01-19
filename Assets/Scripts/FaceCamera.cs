using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Canvas'ın TP'den ne kadar yukarıda duracağını buradan ayarla
    public float yOffset = 0.8f; 

    void LateUpdate()
    {
        if (transform.parent == null) return;

        // 1. POZİSYON: TP ne yöne dönerse dönsün, Canvas'ı dünya koordinatlarında 
        // her zaman TP'nin tam ÜSTÜNE yerleştirir.
        transform.position = transform.parent.position + Vector3.up * yOffset;

        // 2. ROTASYON: Canvas'ın her zaman kameraya bakmasını sağlar.
        // Böylece oyuncu nereden bakarsa baksın butonlar düz görünür.
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                         Camera.main.transform.rotation * Vector3.up);
    }
}