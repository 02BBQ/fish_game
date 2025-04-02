using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.up, 50 * Time.deltaTime);
    }
}