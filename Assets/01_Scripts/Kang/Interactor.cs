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

        outlineMaterial = outlineMesh.sharedMaterials[1];
        materials = outlineMesh.sharedMaterials;
        materials[1] = nullMat;
        outlineMesh.sharedMaterials = materials;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            materials[1] = outlineMaterial;
            outlineMesh.sharedMaterials = materials;
            Definder.Player.AddInteract(OnInteract);
        }
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            materials[1] = nullMat;
            outlineMesh.sharedMaterials = materials;
            Definder.Player.RemoveInterect(OnInteract);
        }
    }
    protected abstract void OnInteract();
}
