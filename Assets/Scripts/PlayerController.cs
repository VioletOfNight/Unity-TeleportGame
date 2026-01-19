using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    private Vector3 moveDirection = Vector3.forward;
    private Rigidbody rb;

    [Header("Stuck (Sıkışma) Ayarları")]
    private float stuckTimer = 0f;
    private Vector3 lastPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isGameStarted) return;

        // 1. HAREKET VE YERÇEKİMİ:
        // 'velocity' yerine 'linearVelocity' kullanıyoruz.
        Vector3 currentVel = rb.linearVelocity;
        
        // X ve Z yönündeki hızımızı biz veriyoruz, Y (düşme) hızını koruyoruz.
        Vector3 targetVel = moveDirection * speed;
        targetVel.y = currentVel.y; 
        
        rb.linearVelocity = targetVel;

        // 2. DÖNÜŞ MANTIĞI:
        if (moveDirection != Vector3.zero)
        {
            rb.rotation = Quaternion.LookRotation(moveDirection);
        }

        // 3. SIKIŞMA (STUCK) KONTROLÜ:
        Vector3 currentPosHorizontal = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 lastPosHorizontal = new Vector3(lastPosition.x, 0, lastPosition.z);
        
        float distanceMoved = Vector3.Distance(currentPosHorizontal, lastPosHorizontal);
        
        if (distanceMoved < 0.01f)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= 1.0f)
            {
                GameManager.Instance.GameOver();
            }
        }
        else
        {
            stuckTimer = 0f; 
        }

        lastPosition = transform.position;
    }

    public void ChangeDirection(Vector3 newDirection)
    {
        moveDirection = new Vector3(newDirection.x, 0, newDirection.z).normalized;
    }
}