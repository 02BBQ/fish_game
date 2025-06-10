using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class FishingVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform fishingRod;
    [SerializeField] private Transform fishingRodTip;
    [SerializeField] private Transform bobber;
    [SerializeField] private ParticleSystem shakeParticleBase;
    [SerializeField] private FishingLineRenderer lineRenderer;

    public Transform Bobber => bobber;
    public Transform FishingRod => fishingRod;
    public Transform FishingRodTip => fishingRodTip;

    private bool isAnchored;
    private Vector3 anchorPosition;
    private ParticleSystem shakeParticle;
    private static readonly Vector3 Zero = Vector3.zero;

    private void Update()
    {
        if (fishingRodTip != null && bobber != null)
        {
            lineRenderer.UpdateLine(fishingRodTip.position, bobber.position);
        }
    }

    public void ResetBobber()
    {
        if (bobber != null)
        {
            bobber.localPosition = new Vector3(-0.0241999999f, -0.056499999f, 0);
            bobber.rotation = Quaternion.identity;
        }
    }

    public void SetAnchor(bool isAnchored, Vector3? anchorPosition = null)
    {
        this.isAnchored = isAnchored;
        this.anchorPosition = anchorPosition ?? Zero;

        if (shakeParticle != null)
        {
            shakeParticle.Stop();
            Destroy(shakeParticle.gameObject, 1f);
        }

        if (isAnchored && shakeParticleBase != null)
        {
            shakeParticle = Instantiate(shakeParticleBase, this.anchorPosition, Quaternion.identity);
            shakeParticle.Play();
        }
    }
}
