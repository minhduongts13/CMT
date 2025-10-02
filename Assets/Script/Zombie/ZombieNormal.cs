using System.Collections;
using UnityEngine;

public class ZombieNormal : Zombie
{
    [Header("Normal Zombie Specific")]
    public bool debugMode = true;

    #region Unity Lifecycle
    protected override void Start()
    {
        base.Start();
    }
    #endregion

    #region Abstract Implementation
    public override void MergeWith(Zombie other)
    {
        if (other is ZombieNormal otherNormal && CanMergeWith(otherNormal))
        {
            // Merge stats
            health += otherNormal.health * 0.5f;
            damage += otherNormal.damage * 0.3f;
            moveSpeed += otherNormal.moveSpeed * 0.2f;

            // Destroy the other zombie
            otherNormal.TakeDamage(9999);

            if (debugMode) Debug.Log($"[ZOMBIE NORMAL] Merged! New HP: {health}, Damage: {damage}");
        }
    }

    public override void Upgrade()
    {
        // Basic upgrade
        health *= 1.2f;
        damage *= 1.1f;
        moveSpeed *= 1.05f;

        if (debugMode) Debug.Log($"[ZOMBIE NORMAL] Upgraded! HP: {health}, Damage: {damage}, Speed: {moveSpeed}");
    }

    public override void Move()
    {
        if (!canMove || isDead || isDragging) return;

        // Simple left movement - THEO ZOMBIECONTROLLER
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        if (debugMode && Time.frameCount % 300 == 0) // Log every 5 seconds
        {
            Debug.Log($"[ZOMBIE NORMAL] Moving at speed: {moveSpeed}, Position: {transform.position.x:F1}");
        }
    }

    public override void Hunt()
    {
        // if (targetPlant == null) return;

        // // Simple hunt behavior - move towards target
        // Vector3 direction = (targetPlant.position - transform.position).normalized;
        // direction.z = 0; // Keep 2D

        // Vector3 huntMovement = direction * moveSpeed * 0.5f * Time.deltaTime;
        // transform.Translate(huntMovement);

        // if (debugMode) Debug.Log($"[ZOMBIE NORMAL] Hunting plant: {targetPlant.name}");
    }

    public override void Skill()
    {
        // Basic skill - temporary speed boost
        if (debugMode) Debug.Log("[ZOMBIE NORMAL] Using skill - Speed Boost!");
        StartCoroutine(SpeedBoost());
    }

    public override void Special()
    {
        // Special ability - temporary rage mode
        if (debugMode) Debug.Log("[ZOMBIE NORMAL] Using special - Bite Frenzy!");
        StartCoroutine(BiteFrenzy());
    }
    #endregion

    #region Skill Abilities
    private IEnumerator SpeedBoost()
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= 1.5f;

        // Visual effect
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.cyan;
        }

        yield return new WaitForSeconds(3f);

        moveSpeed = originalSpeed;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        if (debugMode) Debug.Log("[ZOMBIE NORMAL] Speed boost ended");
    }

    private IEnumerator BiteFrenzy()
    {
        float originalDamage = damage;
        float originalAttackRate = attackRate;

        damage *= 2f;
        attackRate *= 2f;

        // Visual effect
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        yield return new WaitForSeconds(5f);

        damage = originalDamage;
        attackRate = originalAttackRate;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        if (debugMode) Debug.Log("[ZOMBIE NORMAL] Bite frenzy ended");
    }
    #endregion

    #region Utility Methods
    private bool CanMergeWith(ZombieNormal other)
    {
        return other != null &&
               other != this &&
               !other.isDead &&
               other.currentRow == currentRow &&
               Vector3.Distance(transform.position, other.transform.position) <= 2f;
    }
    #endregion

}