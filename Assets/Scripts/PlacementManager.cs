using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // Buton kontrolü için eklendi
using TMPro; // UI metni için eklendi

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    public GameObject tpPrefab;
    public Material ghostMaterial; 
    private Material originalMaterial; 
    
    public GameObject currentTP;
    private bool isInitialPlacement = false;

    public List<Color> tpColors = new List<Color> { Color.magenta, Color.cyan, Color.yellow, Color.green, Color.red };
    private int colorIndex = 0;
    private TeleportPoint lastPlacedTP; 

    // --- Yerleştirilen tüm TP'leri tutan liste ---
    private List<GameObject> allPlacedTPs = new List<GameObject>();

    [Header("TP Limit Ayarları")]
    public int maxTPPairs = 3; // Bölümde izin verilen maksimum çift sayısı
    public Button addTPButton; // "TP Ekle" butonun
    public TextMeshProUGUI tpLimitText; // "TP Hakkı: 0 / 3" yazacak olan metin

    void Awake() { Instance = this; }

    void Start()
    {
        // Oyun başında UI'ı ve buton durumunu güncelle
        UpdateTPUI();
    }

    public void StartPlacingTP()
    {
        // Oyun başladıysa veya zaten bir TP yerleştiriliyorsa çık
        if (GameManager.Instance.isGameStarted || currentTP != null) return;

        // EĞER LİMİT DOLDUYSA YENİ TP OLUŞTURMA
        if (allPlacedTPs.Count >= maxTPPairs * 2) 
        {
            Debug.Log("Maksimum TP limitine ulaşıldı!");
            return;
        }

        currentTP = Instantiate(tpPrefab);
        isInitialPlacement = true;
        Renderer rend = currentTP.GetComponentInChildren<Renderer>();
        if (rend != null) {
            originalMaterial = rend.material;
            rend.material = ghostMaterial;
        }
        currentTP.GetComponent<Collider>().enabled = false;
    }

    void Update()
    {
        if (GameManager.Instance.isGameStarted) return;
        if (isInitialPlacement && currentTP != null) {
            MoveWithMouse();
            if (Input.GetMouseButtonDown(0)) FinalizePlacement();
        }
    }

    void MoveWithMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                if (Mathf.Abs(hit.normal.y) > 0.5f) return;

                Vector3 wallCenter = hit.collider.transform.position;
                float wallHalfSize = hit.collider.bounds.extents.x; 

                currentTP.transform.position = wallCenter + (hit.normal * (wallHalfSize + 0.1f));
                currentTP.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
            else 
            {
                currentTP.transform.position = hit.point + Vector3.up * 0.5f;
                currentTP.transform.rotation = Quaternion.identity;
            }
        }
    }

    void FinalizePlacement()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Wall"))
        {
            isInitialPlacement = false;
            currentTP.GetComponent<Collider>().enabled = true;
            
            TeleportPoint newTP = currentTP.GetComponent<TeleportPoint>();
            Renderer rend = currentTP.GetComponentInChildren<Renderer>();
            Color currentColor = tpColors[colorIndex % tpColors.Count];

            if (rend != null) {
                rend.material = originalMaterial;
                rend.material.color = currentColor;
            }
            
            // --- LİSTEYE EKLE ---
            allPlacedTPs.Add(currentTP);

            if (lastPlacedTP == null) {
                lastPlacedTP = newTP;
            }
            else {
                newTP.pairedExit = lastPlacedTP;
                lastPlacedTP.pairedExit = newTP;
                colorIndex++; 
                lastPlacedTP = null;
            }
            
            //newTP.OpenMenu();
            currentTP = null;

            // YENİ: Yerleştirme bitince UI'ı güncelle
            UpdateTPUI();
        }
    }

    public void UndoPlacement()
    {
        if (GameManager.Instance.isGameStarted || allPlacedTPs.Count == 0) return;

        int lastIndex = allPlacedTPs.Count - 1;
        GameObject toRemove = allPlacedTPs[lastIndex];
        TeleportPoint tpScript = toRemove.GetComponent<TeleportPoint>();
        
        if (tpScript == lastPlacedTP) 
        {
            lastPlacedTP = null;
        }
        else if (tpScript.pairedExit != null) 
        {
            lastPlacedTP = tpScript.pairedExit;
            lastPlacedTP.pairedExit = null;
            colorIndex--;
        }

        allPlacedTPs.RemoveAt(lastIndex);
        Destroy(toRemove);

        // YENİ: Geri alma sonrası UI'ı ve butonun tıklanabilirliğini güncelle
        UpdateTPUI();

        Debug.Log("Son TP geri alındı. Kalan TP sayısı: " + allPlacedTPs.Count);
    }

    // --- YENİ: UI VE BUTON DURUMU GÜNCELLEME ---
    public void UpdateTPUI()
    {
        int currentPairs = allPlacedTPs.Count / 2;
        
        // Metni güncelle (Örn: "TP Hakkı: 1 / 3")
        if (tpLimitText != null)
        {
            tpLimitText.text = "TP Hakkı: " + currentPairs + " / " + maxTPPairs;
        }

        // Eğer limit dolduysa butonu pasifleştir, yer varsa aktifleştir
        if (addTPButton != null)
        {
            addTPButton.interactable = (allPlacedTPs.Count < maxTPPairs * 2);
        }
    }
}