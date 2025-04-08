using System.Collections.Generic;
using UnityEngine;

public class Market : MapEntity, IInteractable
{
    public List<Item> buyItems;
    public List<Item> sellItems;
    public Transform marketUI;
    public Transform[] buyUIParents;
    public Transform[] sellUIParents;
    public GameObject buyGoods;
    public GameObject sellGoods;
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

        foreach (Item item in buyItems)
        {
            AddGoodsInBuy(item);
        }
        foreach (Item item in sellItems)
        {
            AddGoodsInSell(item);
        }
    }

    private void AddGoodsInBuy(Item item)
    {
        BuyGoods copyGoods = Instantiate(buyGoods, buyUIParents[(int)item.type]).GetComponent<BuyGoods>();
        copyGoods.SetItem(item);
    }
    private void AddGoodsInSell(Item item)
    {
        SellGoods copyGoods = Instantiate(sellGoods, sellUIParents[(int)item.type]).GetComponent<SellGoods>();
        copyGoods.SetItem(item);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            materials[1] = outlineMaterial;
            outlineMesh.materials = materials;
            Definder.Player.AddInteract(OnInterect);
            GuideText.Instance.AddGuide("Market");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            materials[1] = nullMat;
            outlineMesh.materials = materials;
            Definder.Player.RemoveInterect(OnInterect);
            GuideText.Instance.RemoveGuide("Market");
        }
    }
    public void OnInterect()
    {
        marketUI.gameObject.SetActive(true);
    }
}
