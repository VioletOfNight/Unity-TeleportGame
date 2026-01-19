using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    public GameObject myCanvas; 

    public void OpenMenu() { }
    public void CloseMenu() { }

    public void RotateTP() { transform.Rotate(0, 0, 90); }

    public void ConfirmPlacement() { 
        CloseMenu(); 
        PlacementManager.Instance.currentTP = null; 
    }

    private void OnMouseDown() {
        if (GameManager.Instance.isGameStarted) return;
        PlacementManager.Instance.currentTP = this.gameObject;
        OpenMenu();
    }
    public TeleportPoint pairedExit; // Bu TP'nin bağlı olduğu diğer TP

    private void OnTriggerEnter(Collider other)
    {
        // Eğer oyun başladıysa ve bir eşleşme varsa ışınla
        if (GameManager.Instance.isGameStarted && pairedExit != null)
        {
            if (other.CompareTag("Player"))
            {
                // Karakteri diğer TP'nin tam önüne ışınla
                // pairedExit.transform.up * 0.5f ekleyerek karakterin duvarın içinde kalmamasını sağlarız
                other.transform.position = pairedExit.transform.position + (pairedExit.transform.up * 0.8f);

                // 2. Karakterin rotasyonunu TP'nin baktığı yöne çevir
                other.transform.rotation = Quaternion.LookRotation(pairedExit.transform.up);
                
                // 3. Karakterin hareket yönünü güncelle
                // Player scriptindeki hareket fonksiyonuna yeni yönü bildiriyoruz
                PlayerController playerMove = other.GetComponent<PlayerController>();
                 if (playerMove != null)
                {
                playerMove.ChangeDirection(pairedExit.transform.up);
                }
                
                // Eğer karakterin hızı varsa korumak veya sıfırlamak isteyebilirsin
                Debug.Log("Işınlanma gerçekleşti!");
            }
        }
    }
}