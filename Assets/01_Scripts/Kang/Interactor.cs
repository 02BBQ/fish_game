using UnityEngine;

public abstract class Interactor : MapEntity
{
    public MeshRenderer outlineMesh;
    public Material nullMat;
    Material outlineMaterial;
    Material[] materials;
    protected override void Start()
    {
        base.Start();

        outlineMaterial = outlineMesh.materials[1];
        materials = outlineMesh.materials;
        materials[1] = nullMat;
        outlineMesh.materials = materials;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            materials[1] = outlineMaterial;
            outlineMesh.materials = materials;
            Definder.Player.AddInteract(OnInterect);
        }
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            materials[1] = nullMat;
            outlineMesh.materials = materials;
            Definder.Player.RemoveInterect(OnInterect);
        }
    }
    protected abstract void OnInterect();
}
