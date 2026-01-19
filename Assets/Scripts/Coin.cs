using UnityEngine;

public class Coin : MonoBehaviour
{
    public string coinID; // Inspector'dan her altına farklı isim ver (L1_C1, L1_C2 gibi)

    void Start()
    {
        // Eğer bu isimde bir kayıt varsa, bu altın daha önce toplanmıştır.
        if (PlayerPrefs.GetInt(coinID, 0) == 1)
        {
            Destroy(gameObject); // O yüzden bu altını sahneden siliyoruz.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Altın toplandığında hafızaya "toplandı" (1) olarak kaydet
            PlayerPrefs.SetInt(coinID, 1);
            PlayerPrefs.Save();

            GameManager.Instance.CollectCoin(1);
            Destroy(gameObject);
        }
    }
}