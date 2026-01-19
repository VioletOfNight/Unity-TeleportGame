using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    public enum AbilityType { None, TransformWall, BreakWall, Hint }
    public AbilityType activeAbility = AbilityType.None;

    [Header("UI Panelleri")]
    public RectTransform drawerPanel;
    private bool isDrawerOpen = false;

    void Awake() { Instance = this; }

    // Çekmeceyi açıp kapatan fonksiyon
    public void ToggleDrawer()
    {
        isDrawerOpen = !isDrawerOpen;
        // Basitçe X pozisyonunu değiştirerek çekmece efekti veriyoruz
        float targetX = isDrawerOpen ? 0f : -200f; 
        drawerPanel.anchoredPosition = new Vector2(targetX, drawerPanel.anchoredPosition.y);
    }

    // Butonlara basınca yetenek seçer
    public void SelectAbility(int type)
    {
        activeAbility = (AbilityType)type;
        Debug.Log("Seçilen Yetenek: " + activeAbility);
    }

    void Update()
    {
        // Eğer bir yetenek seçiliyse ve ekrana tıklanırsa
        if (activeAbility != AbilityType.None && Input.GetMouseButtonDown(0))
        {
            UseAbility();
        }
    }

    void UseAbility()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObj = hit.collider.gameObject;

            // 1. ÖZELLİK: DUVAR DÖNÜŞTÜRME
            if (activeAbility == AbilityType.TransformWall)
            {
                if (clickedObj.CompareTag("PotentialWall"))
                {
                    clickedObj.tag = "Wall";
                    clickedObj.GetComponent<Renderer>().material.color = Color.blue; // Renk değişimi (isteğe bağlı)
                    Debug.Log("Duvar artık ışınlanılabilir!");
                    FinishAbility();
                }
            }
            // 2. ÖZELLİK: DUVAR KIRMA
            else if (activeAbility == AbilityType.BreakWall)
            {
                if (clickedObj.CompareTag("Wall") || clickedObj.CompareTag("PotentialWall"))
                {
                    Destroy(clickedObj);
                    Debug.Log("Duvar kırıldı!");
                    FinishAbility();
                }
            }
        }
    }

    void FinishAbility()
    {
        activeAbility = AbilityType.None; // Yetenek kullanıldıktan sonra seçimi sıfırla
    }
}