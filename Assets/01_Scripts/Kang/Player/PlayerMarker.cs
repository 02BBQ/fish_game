using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    Transform visual;
    private void Start()
    {
        visual = Definder.Player.playerMovement.visual;
    }
    void Update()
    {
        transform.eulerAngles = new Vector3(0f, 0f, -visual.eulerAngles.y + 180);
    }
}
