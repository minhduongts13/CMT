using UnityEngine;

public class testswitchdeck : MonoBehaviour
{
    public void switchdeck()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
