using System.Collections.Generic;
using UnityEngine;

public class Market : Interactor
{
    public List<Item> buyItems;
    public List<Item> sellItems;
    public Transform marketUI;
    public Transform[] buyUIParents;
    public Transform[] sellUIParents;
    public GameObject buyGoods;
    public GameObject sellGoods;

    protected override void Start()
    {
        base.Start();

        foreach (Item item in buyItems)
        {
            AddGoodsInBuy(item);
        }
        foreach (Item item in sellItems)
        {
            AddGoodsInSell(item);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        GuideText.Instance.AddGuide("Market");
    }
    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        GuideText.Instance.RemoveGuide("Market");
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
    protected override void OnInterect()
    {
        marketUI.gameObject.SetActive(true);
    }
}
