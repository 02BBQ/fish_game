using System.Collections.Generic;
using UnityEngine;

public class Market : MapEntity, IInteractable
{
    public List<Item> items;
    public Transform shopUIRoot;
    public Transform shopUIParent;
    public GameObject goods;
    public MeshRenderer outlineMesh;
    Material outlineMaterial;
    Material[] materials;

    protected override void Start()
    {
        base.Start();

        outlineMaterial = outlineMesh.materials[1];
        materials = outlineMesh.materials;
        materials[1] = null;
        outlineMesh.materials = materials;

        foreach (Item item in items)
        {
            Goods copyGoods = Instantiate(goods, shopUIParent).GetComponent<Goods>();
            copyGoods.SetItem(item);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            materials[1] = outlineMaterial;
            outlineMesh.materials = materials;
            Definder.Player.AddInteract(OnInterect);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            materials[1] = null;
            outlineMesh.materials = materials;
            Definder.Player.RemoveInterect(OnInterect);
        }
    }
    public void OnInterect()
    {
        shopUIRoot.gameObject.SetActive(true);
    }
}
