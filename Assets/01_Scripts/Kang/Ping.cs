using UnityEngine;

public class Ping : MonoBehaviour
{
    Transform player;
    Material myMat;
    private void Start()
    {
        player = Definder.Player.transform;
        myMat = GetComponent<Renderer>().material;
    }
    private void Update()
    {
        myMat.SetFloat("_Alpha", Core.Remap(Vector3.Distance(transform.position, player.position), 30f, 65f, 0f, 0.7f));
    }
}
