using System.Collections;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [Header("Row Settings")]
    public int currentRow = -1;
    [Header("Zombie Card Settings")]
    public ZombieCardScriptableObject zombieCardSO;

    [Header("Movement")]
    public float moveSpeed;
    public bool isMoving = true;

    [Header("Combat")]
    public float health;
    public float damage;
    public float attackRange;
    public float attackRate;
    public LayerMask plantLayer;

    [Header("State")]
    public bool isDead = false;
    public bool isAttacking = false;
    public Sprite[] states;
    public int stateCount;

    [Header("Audio")]
    public AudioClip damageAudio;
    public AudioClip attackAudio;
    public AudioClip deathAudio;

    [Header("Effects")]
    public GameObject deathParticles;
    public GameObject attackEffect;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Animator animator;

    public bool isDragging = false; // Cờ để kiểm tra trạng thái kéo thả

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Khởi tạo dữ liệu từ ScriptableObject
        if (zombieCardSO != null)
        {
            LoadZombieData();
        }

        // Bắt đầu di chuyển
        StartCoroutine(MovementCoroutine());
        StartCoroutine(AttackCheckCoroutine());
    }

    private void LoadZombieData()
    {
        health = zombieCardSO.health;
        damage = zombieCardSO.damage;
        moveSpeed = zombieCardSO.moveSpeed;
        attackRange = zombieCardSO.attackRange;
        attackRate = zombieCardSO.attackRate;

        // Cập nhật sprite nếu có
        if (zombieCardSO.zombieIcon != null)
        {
            spriteRenderer.sprite = zombieCardSO.zombieIcon;
        }
    }

    private void Update()
    {
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    // Coroutine di chuyển
    private IEnumerator MovementCoroutine()
    {
        while (!isDead)
        {
            // THÊM: !isDragging check
            if (isMoving && !isAttacking && !isDragging)
            {
                // Di chuyển về phía trái (hướng plant)
                rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);

                if (animator != null)
                {
                    animator.SetBool("isWalking", true);
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

                if (animator != null)
                {
                    animator.SetBool("isWalking", false);
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    // Sửa AttackCheckCoroutine để không dùng Physics2D.OverlapCircle
    private IEnumerator AttackCheckCoroutine()
    {
        while (!isDead)
        {
            if (!isAttacking && !isDragging)
            {
                // TÌM plant trong tầm tấn công bằng distance check
                PlantManager targetPlant = FindNearestPlantInRange();

                if (targetPlant != null)
                {
                    // Bắt đầu tấn công
                    isAttacking = true;
                    isMoving = false;

                    StartCoroutine(AttackCoroutine(targetPlant));
                }
                else
                {
                    // Không có plant trong tầm → tiếp tục di chuyển
                    isAttacking = false;
                    isMoving = true;
                }
            }

            yield return new WaitForSeconds(0.1f); // Check thường xuyên hơn
        }
    }

    // Hàm tìm plant gần nhất trong tầm tấn công
    private PlantManager FindNearestPlantInRange()
    {
        // Tìm tất cả plant trong scene
        PlantManager[] allPlants = FindObjectsByType<PlantManager>(FindObjectsSortMode.None);
        PlantManager nearestPlant = null;
        float nearestDistance = float.MaxValue;

        foreach (PlantManager plant in allPlants)
        {
            if (plant != null && !plant.isDragging)
            {
                Vector3 plantPos = plant.transform.position;
                Vector3 zombiePos = transform.position;

                // Kiểm tra cùng hàng (row)
                float yDifference = Mathf.Abs(plantPos.y - zombiePos.y);
                bool sameRow = yDifference <= 0.5f; // Cho phép sai số

                if (sameRow)
                {
                    // Kiểm tra khoảng cách
                    float distance = Vector2.Distance(zombiePos, plantPos);

                    // Plant phải ở phía trước zombie (X nhỏ hơn)
                    bool plantInFront = plantPos.x < zombiePos.x;

                    if (plantInFront && distance <= attackRange && distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPlant = plant;
                    }
                }
            }
        }

        return nearestPlant;
    }

    // Coroutine thực hiện tấn công
    private IEnumerator AttackCoroutine(PlantManager targetPlant)
    {
        while (isAttacking && targetPlant != null && !isDead)
        {
            // Kiểm tra plant vẫn trong tầm
            float distanceToPlant = Vector2.Distance(transform.position, targetPlant.transform.position);

            if (distanceToPlant <= attackRange)
            {
                // Thực hiện damage
                targetPlant.Damage(damage);

                Debug.Log($"Zombie attacked plant for {damage} damage!");

                // Phát âm thanh tấn công
                if (attackAudio != null && audioSource != null)
                {
                    audioSource.PlayOneShot(attackAudio);
                }

                // Hiệu ứng tấn công
                if (attackEffect != null)
                {
                    Instantiate(attackEffect, targetPlant.transform.position, Quaternion.identity);
                }

                // Animation tấn công
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }

                // Chờ theo attack rate
                yield return new WaitForSeconds(attackRate);
            }
            else
            {
                // Plant ra khỏi tầm → dừng tấn công
                isAttacking = false;
                isMoving = true;
                break;
            }
        }

        // Kết thúc tấn công
        isAttacking = false;
        isMoving = true;
    }

    // Nhận sát thương
    public void Damage(float amount)
    {
        if (isDead) return;

        health -= amount;

        // Phát âm thanh nhận sát thương
        if (damageAudio != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageAudio);
        }

        // Hiệu ứng nhấp nháy khi nhận sát thương
        StartCoroutine(BlinkEffect());

        // Animation nhận sát thương
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }
    }

    // Hiệu ứng nhấp nháy khi nhận sát thương
    private IEnumerator BlinkEffect()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    // Hàm chết
    private void Die()
    {
        isDead = true;
        isMoving = false;
        isAttacking = false;

        // Phát âm thanh chết
        if (deathAudio != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathAudio);
        }

        // Hiệu ứng chết
        if (deathParticles != null)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
        }

        // Animation chết
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Xóa zombie sau một khoảng thời gian
        StartCoroutine(DestroyAfterDelay(1f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // Vẽ gizmos để debug tầm tấn công
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void SetZombieRow(int row)
    {
        // Remove from old row if exists
        if (currentRow >= 0)
        {
            RowOfZombie.Instance.RemoveZombieFromRow(currentRow, this);
        }

        // Add to new row
        currentRow = row;
        RowOfZombie.Instance.AddZombieToRow(row, this);
        Debug.Log($"Zombie set to row {row}");
    }

    private void OnDestroy()
    {
        // Remove from row when destroyed
        if (currentRow >= 0 && RowOfZombie.Instance != null)
        {
            RowOfZombie.Instance.RemoveZombieFromRow(currentRow, this);
        }
    }
}