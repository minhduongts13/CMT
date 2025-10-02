using System.Collections;
using UnityEngine;

public class PeashooterPlant : Plant
{
    [Header("Peashooter Specific")]
    public bool debugMode = true;

    private bool attackRoutineStarted = false;

    #region Unity Lifecycle
    protected override void Start()
    {
        base.Start();
    }
    #endregion

    #region Initialization Override
    protected override void OnPlantInitialized()
    {
        if (debugMode) Debug.Log($"[PEASHOOTER] OnPlantInitialized called");

        FindShootPoint();
        ValidateSetup();
        StartPeashooterAttackRoutine();
    }

    private void ValidateSetup()
    {
        if (debugMode)
        {
            Debug.Log($"[PEASHOOTER] Setup validation:");
            Debug.Log($"  - Bullet: {bullet != null} ({(bullet != null ? bullet.name : "NULL")})");
            Debug.Log($"  - ShootPoint: {shootPoint != null}");
            Debug.Log($"  - CanShoot: {canShoot}");
            Debug.Log($"  - CurrentRow: {currentRow}");
        }
    }

    private void FindShootPoint()
    {
        // Tìm ShootPoint có sẵn
        shootPoint = transform.Find("ShootPoint") ??
                    transform.Find("Muzzle") ??
                    transform.Find("FirePoint");

        // Tạo ShootPoint nếu không có
        if (shootPoint == null)
        {
            GameObject shootPointObj = new GameObject("ShootPoint");
            shootPointObj.transform.SetParent(transform);
            shootPointObj.transform.localPosition = new Vector3(0.5f, 0f, 0f);
            shootPoint = shootPointObj.transform;
        }

        if (debugMode) Debug.Log($"[PEASHOOTER] ShootPoint ready at: {shootPoint.position}");
    }
    #endregion

    #region Attack System
    private void StartPeashooterAttackRoutine()
    {
        if (attackRoutineStarted) return;

        if (canShoot && bullet != null && shootPoint != null)
        {
            attackRoutineStarted = true;
            StartCoroutine(AttackCoroutine()); // ĐỔI TÊN
            if (debugMode) Debug.Log("[PEASHOOTER] Attack routine started!");
        }
        else
        {
            if (debugMode) Debug.LogWarning($"[PEASHOOTER] Cannot start attack: canShoot={canShoot}, bullet={bullet != null}, shootPoint={shootPoint != null}");
        }
    }

    public IEnumerator AttackCoroutine()
    {
        yield return new WaitUntil(() => isInitialized);

        while (!isDead && this != null)
        {
            if (moveSpeed > 0 && !isDragging && shootPoint != null && bullet != null && currentRow >= 0 && isGrown)
            {
                bool hasZombieInRow = RowOfZombie.Instance?.HasZombieInRow(currentRow) ?? false;

                if (hasZombieInRow)
                {
                    Zombie targetZombie = RowOfZombie.Instance.GetFirstZombieInRow(currentRow);

                    if (targetZombie != null)
                    {
                        float distanceX = targetZombie.transform.position.x - shootPoint.position.x;

                        if (distanceX > 0 && distanceX <= attackRange) // Zombie ở phía trước và trong tầm
                        {
                            if (debugMode) Debug.Log($"[PEASHOOTER] Plant in row {currentRow} shooting! Distance: {distanceX}");

                            GameObject bulletInstance = Instantiate(bullet, shootPoint.position, Quaternion.identity);

                            BulletController bulletScript = bulletInstance.GetComponent<BulletController>();
                            if (bulletScript != null)
                            {
                                bulletScript.damage = this.damage;
                                bulletScript.SetTargetRow(currentRow); // GÁN ROW thay vì SetTarget
                            }

                            Rigidbody2D bulletRb = bulletInstance.GetComponent<Rigidbody2D>();
                            if (bulletRb != null)
                            {
                                // BẮN THẲNG theo trục X
                                bulletRb.linearVelocity = Vector2.right * moveSpeed;
                            }

                            if (debugMode) Debug.Log("[PEASHOOTER] Bullet fired!");
                        }
                        else if (debugMode && Time.frameCount % 60 == 0) // Log mỗi giây
                        {
                            Debug.Log($"[PEASHOOTER] Zombie out of range or behind plant. Distance: {distanceX}, Range: {attackRange}");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"hasZombieInRow:{hasZombieInRow}");
                }
            }
            else
            {
                Debug.Log($"moveSpeed: {moveSpeed}, isDragging: {isDragging}, shootPoint: {shootPoint != null}, bullet: {bullet != null}, currentRow: {currentRow}, isGrown: {isGrown}");
            }

            yield return new WaitForSeconds(attackRate);
        }

        attackRoutineStarted = false;
        if (debugMode) Debug.Log("[PEASHOOTER] Attack routine ended");
    }

    // Override Attack method từ Plant base class - GIỮ NGUYÊN
    public override void Attack()
    {
        // Gọi manual attack một lần
        if (bullet != null && shootPoint != null && currentRow >= 0)
        {
            PerformAttack();
        }
    }

    // Override PerformAttack - THỰC SỰ BẮN
    protected override void PerformAttack()
    {
        if (bullet == null || shootPoint == null) return;

        // TẠO BULLET
        GameObject bulletInstance = Instantiate(bullet, shootPoint.position, Quaternion.identity);

        // SETUP BULLETCONTROLLER
        BulletController bulletScript = bulletInstance.GetComponent<BulletController>();
        if (bulletScript != null)
        {
            bulletScript.damage = this.damage;
            bulletScript.SetTargetRow(currentRow);
        }

        // SETUP PHYSICS
        Rigidbody2D bulletRb = bulletInstance.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = Vector2.right * moveSpeed;
        }

        if (debugMode) Debug.Log("[PEASHOOTER] Manual bullet fired!");
    }
    #endregion

    #region Abstract Implementation
    public override void Spawn()
    {
        SetDragging(false);
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(GrowthSequence());
        }
    }

    public override void Fuse(Plant otherPlant)
    {
        if (otherPlant is PeashooterPlant other && CanFuseWith(other))
        {
            damage += other.damage * 0.5f;
            attackRate *= 1.2f;
            other.TakeDamage(9999);
        }
    }

    public override void Idle()
    {
        // Animation logic if needed
    }

    public override void Skill()
    {
        ApplyBoost(1.2f, 3f, 5f);
    }

    public override void Special()
    {
        StartCoroutine(TripleShot());
    }

    private IEnumerator TripleShot()
    {
        for (int i = 0; i < 3; i++)
        {
            if (this == null || isDragging) yield break;
            PerformAttack();
            yield return new WaitForSeconds(0.15f);
        }
    }
    #endregion
}