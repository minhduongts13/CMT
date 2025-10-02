
// using UnityEngine;

// public abstract class PlacableObject : MonoBehaviour
// {
//     private float damage;
//     private float cost;
//     private string name;
//     private UnityEngine.Vector2 position;
//     private string state;
//     private float attackSpeed;
//     private string owner;
//     private float spawnCooldown;
//     private bool isAvailable;

//     public abstract void Attack();
//     public abstract void Spawn();
//     public abstract void Skill();
//     public abstract void Special();
// }
using System.Collections;
using UnityEngine;

public abstract class PlacableObject : MonoBehaviour
{
    #region Common Stats - Từ cả Plant và Zombie
    [Header("Base Stats")]
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackRate; // Tương đương fireRate (Plant) và attackRate (Zombie)

    [Header("Position")]
    [SerializeField] protected int currentRow = -1;

    [Header("State")]
    [SerializeField] protected bool isDead = false;
    [SerializeField] protected bool isDragging = false;
    [SerializeField] protected bool isAttacking = false;
    [SerializeField] protected bool isMoving = true;
    [SerializeField] protected bool isInitialized = false; // Flag để kiểm tra đã load xong stats từ SO chưa
    #endregion

    #region Common Components - Cả Plant và Zombie đều có
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected AudioSource audioSource;
    protected Animator animator;
    #endregion

    #region Common Audio - Cả hai đều có damage/death audio
    [Header("Audio")]
    public AudioClip damageAudio;
    public AudioClip deathAudio;
    #endregion

    #region Unity Lifecycle
    protected virtual void Awake()
    {
        InitializeComponents();
    }

    protected virtual void Start()
    {
        Initialize();
    }

    protected virtual void Update()
    {
        if (isInitialized && health <= 0 && !isDead)
        {
            Debug.Log($"{name} has died.");
            Die();
        }
    }
    #endregion

    #region Initialization
    protected virtual void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Initialize()
    {
        // Override in subclasses
    }
    #endregion

    #region Abstract Methods - MUST IMPLEMENT
    /// <summary>
    /// Main attack behavior
    /// </summary>
    public abstract void Attack();
    /// <summary>
    /// Spawn/placement logic - THÊM DÒNG NÀY
    /// </summary>
    public abstract void Spawn();

    /// <summary>
    /// Special ability/skill - THÊM NẾU CHƯA CÓ
    /// </summary>
    public abstract void Skill();

    /// <summary>
    /// Ultimate special ability - THÊM NẾU CHƯA CÓ
    /// </summary>
    public abstract void Special();

    /// <summary>
    /// Load stats from ScriptableObject
    /// </summary>
    protected abstract void LoadStatsFromSO();

    /// <summary>
    /// Main behavior update
    /// </summary>
    public abstract void UpdateBehavior();
    #endregion

    #region Common Damage System - Cả Plant và Zombie
    /// <summary>
    /// Take damage - common cho cả Plant và Zombie
    /// </summary>
    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;

        // Common damage feedback
        if (damageAudio != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageAudio);
        }

        StartCoroutine(DamageEffect());

        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }
    }

    /// <summary>
    /// Death logic - common cho cả Plant và Zombie
    /// </summary>
    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        isMoving = false;
        isAttacking = false;

        // Death sound
        if (deathAudio != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathAudio);
        }

        // Death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        OnDeath();
        Debug.Log($"{name} has died.");

        // Destroy after delay
        StartCoroutine(DestroyAfterDelay(1f));
    }

    protected virtual void OnDeath()
    {
        // Override for custom death effects
        StopAllCoroutines();
    }

    protected virtual IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    #endregion

    #region Common Effects
    protected virtual IEnumerator DamageEffect()
    {
        // Blink effect khi nhận damage - cả Plant và Zombie đều có
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }
    #endregion

    #region Row Management - Cả Plant và Zombie đều dùng row system
    /// <summary>
    /// Set current row
    /// </summary>
    public virtual void SetRow(int row)
    {
        currentRow = row;
        OnRowChanged(row);
    }

    protected virtual void OnRowChanged(int row)
    {
        // Override in subclasses
        Debug.Log($"{name} set to row {row}");
    }
    #endregion

    #region Common Utility
    /// <summary>
    /// Check if can perform action based on rate
    /// </summary>
    public virtual bool CanPerformAction()
    {
        return !isDead && !isDragging;
    }

    /// <summary>
    /// Set dragging state
    /// </summary>
    public virtual void SetDragging(bool dragging)
    {
        isDragging = dragging;
    }

    /// <summary>
    /// Distance check
    /// </summary>
    public virtual float DistanceTo(Transform target)
    {
        return Vector2.Distance(transform.position, target.position);
    }
    #endregion

    #region Properties
    public float Health => health;
    public float Damage => damage;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackRate => attackRate;
    public int CurrentRow
    {
        get { return currentRow; }
        set { currentRow = value; }
    }
    public bool IsDead => isDead;
    public bool IsDragging
    {
        get { return isDragging; }
        set { isDragging = value; }
    }
    public bool IsAttacking => isAttacking;
    public bool IsMoving => isMoving;
    #endregion

    #region Debug
    protected virtual void OnDrawGizmosSelected()
    {
        // Draw attack range
        if (attackRange > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
    #endregion
}