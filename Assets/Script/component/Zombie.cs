
// public abstract class Zombie : PlacableObject
// {
//     public abstract void MergeWith(Zombie other);
//     public abstract void Upgrade();
// }
using System.Collections;
using UnityEngine;

public abstract class Zombie : PlacableObject
{
    #region Zombie Properties
    [Header("Zombie Specific")]
    [SerializeField] protected ZombieCardScriptableObject zombieCardSO;
    [SerializeField] protected bool canMove = true;

    [Header("Movement")]
    [SerializeField] protected Vector3 moveDirection = Vector3.left;

    protected bool behaviorRoutineStarted = false;
    protected Plant targetPlant;
    #endregion

    #region Initialization
    protected override void Initialize()
    {
        if (zombieCardSO != null)
        {
            LoadStatsFromSO();
        }
        else
        {
            StartCoroutine(WaitForInitialization());
        }
    }
    protected virtual IEnumerator WaitForInitialization()
    {
        Debug.Log("[ZOMBIE] Waiting for zombieCardSO...");
        yield return new WaitUntil(() => zombieCardSO != null);
        LoadStatsFromSO();
    }
    protected override void LoadStatsFromSO()
    {
        Debug.Log("[ZOMBIE] LoadStatsFromSO() CALLED!");

        if (zombieCardSO == null)
        {
            Debug.LogError("[ZOMBIE] zombieCardSO is null in LoadStatsFromSO!");
            return;
        }

        health = zombieCardSO.ToughNess;
        damage = zombieCardSO.Damage;
        moveSpeed = zombieCardSO.MoveSpeed;
        attackRange = zombieCardSO.AttackRange;
        attackRate = zombieCardSO.AttackRate;

        isInitialized = true;
        Debug.Log($"[ZOMBIE] INITIALIZED! Stats - HP: {health}, Damage: {damage}, Speed: {moveSpeed}");
        Debug.Log($"[ZOMBIE] canMove: {canMove}, isMoving: {isMoving}, isDragging: {isDragging}");

        OnZombieInitialized();
    }

    protected virtual void OnZombieInitialized()
    {
        if (!behaviorRoutineStarted)
        {
            StartZombieBehaviorRoutine();
        }
    }
    #endregion

    #region Behavior System
    private void StartZombieBehaviorRoutine()
    {
        if (behaviorRoutineStarted) return;

        behaviorRoutineStarted = true;
        StartCoroutine(MovementCoroutine());
        StartCoroutine(AttackCheckCoroutine());
    }

    private IEnumerator MovementCoroutine()
    {
        // WAIT UNTIL INITIALIZED - QUAN TRỌNG!
        yield return new WaitUntil(() => isInitialized);

        Debug.Log("[ZOMBIE] MovementCoroutine initialized, starting movement...");

        while (!isDead)
        {
            if (isMoving && !isAttacking && !isDragging)
            {
                Move(); // Abstract method
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator AttackCheckCoroutine()
    {
        while (!isDead)
        {
            if (!isAttacking && !isDragging)
            {
                Plant nearestPlant = FindNearestPlantInRange();

                if (nearestPlant != null)
                {
                    // Bắt đầu tấn công
                    isAttacking = true;
                    isMoving = false;
                    targetPlant = nearestPlant;

                    StartCoroutine(AttackCoroutine(nearestPlant));
                }
                else
                {
                    // Không có plant trong tầm → tiếp tục di chuyển
                    isAttacking = false;
                    isMoving = true;
                    targetPlant = null;
                }
                Debug.Log($"[ZOMBIE] Attack check: NearestPlant={(nearestPlant != null ? nearestPlant.name : "None")}, IsAttacking={isAttacking}, IsMoving={isMoving}");
            }
            else
            {
                Debug.Log("[ZOMBIE] Currently attacking or dragging, skipping attack check.");
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private Plant FindNearestPlantInRange()
    {
        Plant[] allPlants = FindObjectsByType<Plant>(FindObjectsSortMode.None);
        Plant nearestPlant = null;
        float nearestDistance = float.MaxValue;

        foreach (Plant plant in allPlants)
        {
            if (plant != null && !plant.IsDragging)
            {
                Vector3 plantPos = plant.transform.position;
                Vector3 zombiePos = transform.position;

                float yDifference = Mathf.Abs(plantPos.y - zombiePos.y);
                bool sameRow = yDifference <= 0.5f; // Cho phép sai số

                if (sameRow)
                {
                    float distance = Vector2.Distance(zombiePos, plantPos);

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

    private IEnumerator AttackCoroutine(Plant targetPlant)
    {
        while (isAttacking && targetPlant != null && !isDead)
        {
            float distanceToPlant = Vector2.Distance(transform.position, targetPlant.transform.position);

            if (distanceToPlant <= attackRange)
            {
                targetPlant.TakeDamage(damage);

                Debug.Log($"Zombie attacked plant for {damage} damage!");

                // Hunt behavior
                Hunt();

                yield return new WaitForSeconds(attackRate);
            }
            else
            {
                isAttacking = false;
                isMoving = true;
                break;
            }
        }

        isAttacking = false;
        isMoving = true;
    }
    #endregion

    #region PlacableObject Implementation
    public override void UpdateBehavior()
    {
        // Handled by coroutines
    }

    public override void Attack()
    {
        // Handled by AttackCoroutine
    }

    public override void Spawn()
    {
        SetDragging(false);
        isMoving = true;

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(SpawnSequence());
        }
    }

    protected virtual IEnumerator SpawnSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (isInitialized && !behaviorRoutineStarted)
        {
            StartZombieBehaviorRoutine();
        }
    }
    #endregion

    #region Abstract Methods
    public abstract void MergeWith(Zombie other);
    public abstract void Upgrade();
    public abstract void Move();
    public abstract void Hunt();
    public abstract override void Skill();
    public abstract override void Special();
    #endregion

    #region Utility Methods
    public virtual void SetZombieCardSO(ZombieCardScriptableObject so)
    {
        zombieCardSO = so;
        if (so != null)
        {
            LoadStatsFromSO();
        }
    }

    public virtual void SetZombieRow(int row)
    {
        SetRow(row);
        RowOfZombie.Instance?.AddZombieToRow(row, this);
    }

    public virtual bool HasReachedEnd()
    {
        return transform.position.x <= -10f;
    }
    #endregion

    #region Properties
    public bool CanMove => canMove;
    public Plant TargetPlant => targetPlant;
    #endregion
    public ZombieCardScriptableObject ZombieCardSO
    {
        get { return zombieCardSO; }
        set { zombieCardSO = value; }
    }
}