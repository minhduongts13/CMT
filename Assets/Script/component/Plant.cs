
// public abstract class Plant : PlacableObject
// {
//     private float attackRange;

//     public abstract void Fuse(Plant otherPlant);
//     public abstract void Idle();
// }
using System.Collections;
using UnityEngine;

public abstract class Plant : PlacableObject
{
    #region Plant Specific Properties
    [Header("Plant Specific")]
    [SerializeField] protected PlantCardScriptableObject plantCardSO;
    [SerializeField] protected Transform shootPoint;
    [SerializeField] protected GameObject bullet;
    [SerializeField] protected LayerMask zombieLayer;
    [SerializeField] protected bool canShoot = true;
    [SerializeField] protected bool isGrown = false;

    [Header("Plant Effects")]
    public GameObject deathParticles;
    #endregion

    #region Initialization
    protected override void Initialize()
    {
        if (plantCardSO != null)
        {
            LoadStatsFromSO();
        }
        else
        {
            StartCoroutine(WaitForInitialization());
        }
    }

    protected override void LoadStatsFromSO()
    {
        if (plantCardSO == null) return;

        health = plantCardSO.ToughNess;
        damage = plantCardSO.Damage;
        attackRange = plantCardSO.Range;
        moveSpeed = plantCardSO.SpeedBullet; // Bullet speed
        attackRate = plantCardSO.FireRate;
        bullet = plantCardSO.Bullet;

        isInitialized = true;

        Debug.Log($"[PLANT] {plantCardSO.Name} loaded stats - HP: {health}, Damage: {damage}, Range: {attackRange}");

        OnPlantInitialized();
    }

    protected virtual IEnumerator WaitForInitialization()
    {
        Debug.Log("[PLANT] Waiting for plantCardSO...");
        yield return new WaitUntil(() => plantCardSO != null);
        LoadStatsFromSO();
    }

    protected virtual void OnPlantInitialized()
    {
        // Override in subclasses
        if (canShoot && bullet != null)
        {
            StartCoroutine(AttackRoutine());
        }
    }
    #endregion

    #region PlacableObject Implementation
    public override void UpdateBehavior()
    {
        if (!isInitialized || !isGrown) return;

        if (HasZombieInRange())
        {
            isAttacking = true;
            // Attack handled by AttackRoutine for consistent timing
        }
        else
        {
            isAttacking = false;
            Idle();
        }
    }

    public override void Attack()
    {
        if (!CanPerformAction() || !canShoot || !isGrown) return;

        if (HasZombieInRange())
        {
            PerformAttack();
        }
    }
    #endregion

    #region Plant Abstract Methods - MUST IMPLEMENT
    /// <summary>
    /// Fusion logic with another plant
    /// </summary>
    public abstract void Fuse(Plant otherPlant);

    /// <summary>
    /// Idle behavior when no enemies around
    /// </summary>
    public abstract void Idle();

    /// <summary>
    /// Plant growth/spawn logic
    /// </summary>
    public abstract override void Spawn();

    /// <summary>
    /// Plant special skill
    /// </summary>
    public abstract override void Skill();

    /// <summary>
    /// Plant ultimate ability
    /// </summary>
    public abstract override void Special();
    #endregion

    #region Combat System
    protected virtual bool HasZombieInRange()
    {
        if (currentRow < 0) return false;

        // Check using RowOfZombie system
        bool hasZombie = RowOfZombie.Instance?.HasZombieInRow(currentRow) ?? false;
        if (!hasZombie) return false;

        // Check distance to first zombie in row
        Zombie targetZombie = RowOfZombie.Instance.GetFirstZombieInRow(currentRow);
        if (targetZombie == null) return false;

        float distance = DistanceTo(targetZombie.transform);
        return distance <= attackRange;
    }

    protected virtual void PerformAttack()
    {
        if (bullet != null && shootPoint != null)
        {
            GameObject bulletInstance = Instantiate(bullet, shootPoint.position, shootPoint.rotation);

            // Set bullet properties
            BulletController bulletScript = bulletInstance.GetComponent<BulletController>();
            if (bulletScript != null)
            {
                bulletScript.damage = this.damage;
                bulletScript.SetTargetRow(currentRow);
            }

            // Set bullet velocity
            Rigidbody2D bulletRb = bulletInstance.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = Vector2.right * moveSpeed; // Use moveSpeed as bullet speed
            }

            OnAttackPerformed();
        }
    }

    protected virtual void OnAttackPerformed()
    {
        // Override for attack effects
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    protected virtual IEnumerator AttackRoutine()
    {
        while (!isDead && isInitialized)
        {
            if (isGrown && !isDragging && canShoot)
            {
                Attack();
            }

            yield return new WaitForSeconds(1f / attackRate);
        }
    }
    #endregion

    #region Plant Growth System
    protected virtual IEnumerator GrowthSequence()
    {
        // Growth animation
        float growthTime = 1f;

        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < growthTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / growthTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
        isGrown = true;

        OnGrowthComplete();
    }

    protected virtual void OnGrowthComplete()
    {
        Debug.Log($"[PLANT] {name} growth complete");
    }
    #endregion

    #region Death Override
    protected override void OnDeath()
    {
        base.OnDeath();

        // Stop attack routine
        StopAllCoroutines();

        // Death effects
        if (deathParticles != null)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
        }

        OnPlantDeath();
    }

    protected virtual void OnPlantDeath()
    {
        // Override for plant-specific death effects
        Debug.Log($"[PLANT] {name} died");
    }
    #endregion

    #region Plant Utility Methods
    /// <summary>
    /// Set plant card data
    /// </summary>
    public virtual void SetPlantCardSO(PlantCardScriptableObject so)
    {
        plantCardSO = so;

        if (!isInitialized)
        {
            LoadStatsFromSO();
        }
    }

    /// <summary>
    /// Set plant row for combat
    /// </summary>
    public virtual void SetPlantRow(int row)
    {
        SetRow(row);
    }

    /// <summary>
    /// Enable/disable shooting
    /// </summary>
    public virtual void SetCanShoot(bool canShoot)
    {
        this.canShoot = canShoot;
    }

    /// <summary>
    /// Check if can fuse with another plant
    /// </summary>
    public virtual bool CanFuseWith(Plant otherPlant)
    {
        return otherPlant != null &&
               otherPlant != this &&
               !otherPlant.isDead &&
               otherPlant.isGrown;
    }

    /// <summary>
    /// Apply temporary boost to plant
    /// </summary>
    public virtual void ApplyBoost(float damageMultiplier, float speedMultiplier, float duration)
    {
        StartCoroutine(BoostCoroutine(damageMultiplier, speedMultiplier, duration));
    }

    protected virtual IEnumerator BoostCoroutine(float dmgMult, float spdMult, float duration)
    {
        float originalDamage = damage;
        float originalSpeed = attackRate;

        damage *= dmgMult;
        attackRate *= spdMult;

        Debug.Log($"[PLANT] {name} boosted! Damage: {damage}, Speed: {attackRate}");

        yield return new WaitForSeconds(duration);

        damage = originalDamage;
        attackRate = originalSpeed;

        Debug.Log($"[PLANT] {name} boost expired");
    }
    #endregion

    #region Properties
    public PlantCardScriptableObject PlantCardSO => plantCardSO;
    public Transform ShootPoint => shootPoint;
    public GameObject Bullet => bullet;
    public bool CanShoot => canShoot;
    public bool IsGrown => isGrown;
    public bool IsInitialized => isInitialized;
    public LayerMask ZombieLayer => zombieLayer;
    #endregion

    #region Debug
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Draw shoot point
        if (shootPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(shootPoint.position, Vector3.one * 0.2f);

            // Draw shooting direction
            if (attackRange > 0)
            {
                Gizmos.DrawLine(shootPoint.position,
                    shootPoint.position + Vector3.right * attackRange);
            }
        }

        // Draw row indicator
        if (currentRow >= 0)
        {
            Gizmos.color = Color.cyan;
            Vector3 pos = transform.position;
            pos.y = currentRow;
            Gizmos.DrawWireCube(pos, new Vector3(0.5f, 0.1f, 0.5f));
        }
    }
    #endregion
}