using UnityEngine;

public class BoatEdge : MonoBehaviour
{
    public bool CheckEdge(float distance)
    {
        return Physics.Raycast(transform.position, Vector3.down, distance);
    }
}
