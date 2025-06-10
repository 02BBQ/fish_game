using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f;
    private bool isRotating = true;

    private void Update()
    {
        if (isRotating)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public void StartRotation()
    {
        isRotating = true;
    }

    public void StopRotation()
    {
        isRotating = false;
    }

    public void Reset()
    {
        transform.rotation = Quaternion.identity;
    }
}