using UnityEngine;

public class MapCamera : MonoBehaviour
{
    void Update()
    {
        Vector3 playerPos = Definder.Player.transform.position;
        playerPos.y = 500f;
        transform.position = playerPos;
    }
}
