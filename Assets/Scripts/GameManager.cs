using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isGameStarted = false;
    public GameObject startButton;

    [Header("Coin Ayarları")]
    public int currentLevelCoins = 0; // Sadece bu turda toplanan (Henüz cüzdanda değil)
    public int totalCoins = 0;        // Tüm oyun boyunca biriken
    public TextMeshProUGUI coinText; 

    [Header("Anahtar Sistemi")]
    public int totalKeysInLevel;
    public int collectedKeys = 0;
    public TextMeshProUGUI keyText;

    [Header("Paneller")]
    public GameObject gameOverPanel; 
    public GameObject winPanel; // Yeni ekledik: Win Paneli
    public TextMeshProUGUI winCoinText; // Paneldeki "X Altın Topladın" yazısı

    void Awake() 
    { 
        Instance = this; 
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        UpdateCoinUI();
    }

    void Start() { UpdateKeyUI(); }

    public void StartGame()
    {
        isGameStarted = true;
        startButton.SetActive(false);
    }

    public void AddKey()
    {
        collectedKeys++;
        UpdateKeyUI();
    }

    public void UpdateKeyUI()
    {
        if (keyText != null)
            keyText.text = "Anahtarlar: " + collectedKeys + " / " + totalKeysInLevel;
    }

    // --- KAZANMA VE KAYBETME SİSTEMİ ---

    public void LevelComplete()
    {
        if (!isGameStarted) return;
        isGameStarted = false;

        // 1. Paraları şimdi cüzdana aktar ve kaydet (SADECE KAZANINCA)
        totalCoins += currentLevelCoins;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();

        // 2. Win panelini göster
        if(winPanel != null)
        {
            winPanel.SetActive(true);
            if(winCoinText != null) winCoinText.text = "Toplanan Altın: " + currentLevelCoins;
        }
    }

    public void GameOver()
    {
        if (!isGameStarted) return;
        isGameStarted = false;
        if(gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    // --- BUTON FONKSİYONLARI (Hem Win hem Loss paneli kullanabilir) ---

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        // Bir sonraki sahneye geçer
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // --- COIN TOPLAMA (DEĞİŞTİ!) ---

    public void CollectCoin(int amount)
    {
        // Artık sadece bölüm içindeki sayacı artırıyoruz, totalCoins'e ellemiyoruz.
        currentLevelCoins += amount;
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "Altın: " + currentLevelCoins.ToString();
    }
}