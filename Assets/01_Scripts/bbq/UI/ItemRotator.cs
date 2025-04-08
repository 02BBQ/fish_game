using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.up, 50 * Time.deltaTime);
    }

    public void Reset()
    {
        transform.rotation = Quaternion.Euler(0,180f,0);
    }
}