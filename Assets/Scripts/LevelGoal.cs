using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.collectedKeys >= GameManager.Instance.totalKeysInLevel)
            {
                GameManager.Instance.LevelComplete();
            }
            else
            {
                Debug.Log("Eksik anahtar var!");
            }
        }
    }
}