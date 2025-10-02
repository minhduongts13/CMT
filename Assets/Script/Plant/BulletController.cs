using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float damage = 25f;
    public float lifeTime = 5f;
    public int targetRow = -1; // Row mà đạn này nhắm vào

    private bool hasHit = false;

    private void Start()
    {
        // Tự hủy đạn sau thời gian
        Destroy(gameObject, lifeTime);
        Debug.Log($"Bullet created for row {targetRow}");
    }

    private void Update()
    {
        // Hủy đạn nếu bay quá xa
        if (transform.position.x > 20f)
        {
            Debug.Log("Bullet destroyed - out of bounds");
            Destroy(gameObject);
            return;
        }

        // Kiểm tra va chạm với zombie trong cùng row
        if (!hasHit && targetRow >= 0)
        {
            CheckHitZombieInRow();
        }
    }

    private void CheckHitZombieInRow()
    {
        // Lấy zombie đầu tiên (gần nhất) trong row
        Zombie targetZombie = RowOfZombie.Instance.GetFirstZombieInRow(targetRow);

        if (targetZombie != null && !targetZombie.IsDragging)
        {
            Vector3 bulletPos = transform.position;
            Vector3 zombiePos = targetZombie.transform.position;

            // CHỈ kiểm tra khoảng cách theo trục X
            float distanceX = Mathf.Abs(zombiePos.x - bulletPos.x);

            Debug.Log($"Bullet X: {bulletPos.x}, Zombie X: {zombiePos.x}, Distance X: {distanceX}");

            // Nếu đạn đủ gần zombie (trong phạm vi 0.5 unit theo X) và cùng hàng
            if (distanceX <= 0.5f)
            {
                Debug.Log($"Bullet hit zombie {targetZombie.name} in row {targetRow}!");

                // Gây sát thương
                targetZombie.TakeDamage(damage);
                hasHit = true;

                // Hủy đạn
                Destroy(gameObject);
                return;
            }
        }
    }

    public void SetTargetRow(int row)
    {
        targetRow = row;
        Debug.Log($"Bullet targeting row: {row}");
    }
}