using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlantManager : MonoBehaviour
{
    [Header("Row Settings")]
    public int plantRow = -1;
    [Header("Plant Card Settings")]
    public PlantCardScriptableObject plantCardSO;

    [Header("Shooting")]
    public Transform shootPoint;
    public GameObject bullet;

    [Header("Stats")]
    public float health;
    public float damage;
    public float range;
    public float speed;
    public LayerMask zombieLayer;
    public float fireRate;

    [Header("State")]
    public bool isMine;
    public float growDuration;
    public GameObject explosion;
    public Sprite grownSprite;
    public float blinkRate;
    public Sprite[] states;
    public int stateCount;
    public bool isGrown;

    [Header("Audio")]
    public AudioClip damageAudio;

    [Header("Effects")]
    public GameObject mineParticles;

    public bool isDragging = true;

    private void Start()
    {
        StartCoroutine(WaitForInitialization());
    }
    private bool isInitialized = false; // Thêm flag
    private IEnumerator WaitForInitialization()
    {
        yield return new WaitUntil(() => plantCardSO != null);

        health = plantCardSO.health;
        damage = plantCardSO.damage;
        range = plantCardSO.range;
        speed = plantCardSO.bulletSpeed;
        fireRate = plantCardSO.fireRate;
        bullet = plantCardSO.bullet;
        zombieLayer = plantCardSO.zombieLayer;
        isInitialized = true;

        Debug.Log($"Plant initialized - Health: {health}, Damage: {damage}, Range: {range}");

        // Bật lại Attack
        StartCoroutine(Attack());

        // if (isMine)
        // {
        //     StartCoroutine(mineStateUpdate());
        // }

        // StartCoroutine(Attack());
    }

    private void Update()
    {
        // Chỉ kiểm tra health khi đã có plantCardSO
        if (isInitialized && health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    // Hàm tấn công sử dụng Grid System
    // Trong PlantManager.Attack(), sửa phần tạo bullet:
    public IEnumerator Attack()
    {
        yield return new WaitUntil(() => isInitialized);

        while (true)
        {
            if (speed > 0 && !isDragging && shootPoint != null && bullet != null && plantRow >= 0)
            {
                // Kiểm tra có zombie trong cùng row không
                bool hasZombieInRow = RowOfZombie.Instance.HasZombieInRow(plantRow);

                if (hasZombieInRow)
                {
                    // Lấy zombie đầu tiên để kiểm tra range
                    ZombieController targetZombie = RowOfZombie.Instance.GetFirstZombieInRow(plantRow);

                    if (targetZombie != null)
                    {
                        // Tính khoảng cách chỉ theo trục X
                        float distanceX = targetZombie.transform.position.x - shootPoint.position.x;

                        if (distanceX > 0 && distanceX <= range) // Zombie ở phía trước và trong tầm
                        {
                            Debug.Log($"Plant in row {plantRow} shooting! Distance: {distanceX}");

                            GameObject bulletInstance = Instantiate(this.bullet, shootPoint.position, Quaternion.identity);

                            BulletController bulletScript = bulletInstance.GetComponent<BulletController>();
                            if (bulletScript != null)
                            {
                                bulletScript.damage = this.damage;
                                bulletScript.SetTargetRow(plantRow); // GÁN ROW thay vì SetTarget
                            }

                            Rigidbody2D bulletRb = bulletInstance.GetComponent<Rigidbody2D>();
                            if (bulletRb != null)
                            {
                                // BẮN THẲNG theo trục X
                                bulletRb.linearVelocity = Vector2.right * speed;
                            }
                        }
                        else
                        {
                            Debug.Log($"Zombie out of range or behind plant. Distance: {distanceX}, Range: {range}");
                        }
                    }
                }
            }

            yield return new WaitForSeconds(fireRate);
        }
    }

    // Hàm set row cho plant
    public void SetPlantRow(int row)
    {
        plantRow = row;
        Debug.Log($"Plant set to row {row}");
    }


    // Hàm cập nhật trạng thái mine
    private IEnumerator mineStateUpdate()
    {
        isGrown = false;
        yield return new WaitUntil(() => !isDragging);
        yield return new WaitForSeconds(growDuration);
        isGrown = true;
    }

    // Hàm nhấp nháy
    // public IEnumerator blink()
    // {
    //     yield return new WaitUntil(() => !isDragging);
    //     this.GetComponent<SpriteRenderer>().sprite = states[stateCount];
    //     yield return new WaitForSeconds(blinkRate);
    //     stateCount = isGrown ? stateCount == 2 ? 3 : 2 : stateCount == 1 ? 0 : 1;
    //     StartCoroutine(blink());
    // }

    // Xử lý va chạm
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMine && isGrown)
        {
            if (collision.CompareTag("Zombie"))
            {
                mineParticles.SetActive(true);
                collision.GetComponent<ZombieController>().Damage(damage);
                Destroy(this.gameObject, 1.5f);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isMine && isGrown)
        {
            if (collision.CompareTag("Zombie"))
            {
                Instantiate(explosion, this.transform);
                Destroy(this.gameObject);
            }
        }
    }

    // Nhận sát thương
    public void Damage(float amt)
    {
        if (!this.GetComponent<AudioSource>().isPlaying)
        {
            this.GetComponent<AudioSource>().Play();
        }
        health -= amt;
    }
}