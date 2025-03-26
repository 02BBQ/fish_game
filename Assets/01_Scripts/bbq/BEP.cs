using UnityEngine;

public class BEP : MonoBehaviour
{
    [SerializeField] private ParticleSystem attention;

    public void PlayAttention()
    {
        attention.Play();
    }
}
