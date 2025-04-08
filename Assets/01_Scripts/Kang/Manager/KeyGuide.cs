using UnityEngine;

public class KeyGuide : MonoBehaviour
{
    public GameObject key;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            key.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            key.SetActive(false);
    }
}
