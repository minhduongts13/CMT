using UnityEngine;

public class FitParent : MonoBehaviour
{
    void Start()
    {
        Transform parent = transform.parent;
        if (parent != null)
        {
            // Đặt vị trí con = vị trí cha
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // Tính ngược lại scale để vừa khít cha
            Vector3 parentScale = parent.lossyScale;
            transform.localScale = new Vector3(
                1f / parentScale.x,
                1f / parentScale.y,
                1f / parentScale.z
            );
        }
    }
}
