using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
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

    List<SellGoods> initItems;

    private void Awake()
    {

        initItems = new List<SellGoods>();

    }
    protected override void Start()
    {
        base.Start();
        foreach (Item item in buyItems)
        {
            AddGoodsInBuy(item);
        }
    }
    private void OnEnable()
    {
        EventManager.AddListener<AddItemEvent>(AddGoodsInSell);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<AddItemEvent>(AddGoodsInSell);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            base.OnTriggerEnter(other);
            GuideText.Instance.AddGuide("Market");
        }
    }
    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            base.OnTriggerExit(other);
            GuideText.Instance.RemoveGuide("Market");
        }
    }

    private void AddGoodsInBuy(Item item)
    {
        BuyGoods copyGoods = Instantiate(buyGoods, buyUIParents[(int)item.type]).GetComponent<BuyGoods>();
        copyGoods.SetItem(item);
    }
    private void AddGoodsInSell(AddItemEvent addItemEvent)
    {
        foreach (Item addItem in addItemEvent.getItems)
        {
            print(addItem.nameStr);
            print(initItems.Count);
            bool isItemInInit = initItems.Any(initItem => initItem.item.nameStr == addItem.nameStr);
            if (!isItemInInit)
            {
                SellGoods copyGoods = Instantiate(sellGoods, sellUIParents[(int)addItem.type]).GetComponent<SellGoods>();
                copyGoods.SetItem(addItem);
                initItems.Add(copyGoods);
            }
            else
            {
                initItems.First(initItem => initItem.item.nameStr == addItem.nameStr).gameObject.SetActive(true);
            }
        }
        foreach (SellGoods initItem in initItems)
        {
            bool isItemInAdd = addItemEvent.getItems.Any(addItem => addItem.nameStr == initItem.item.nameStr);
            if (!isItemInAdd)
            {
                initItem.gameObject.SetActive(false);
            }
        }
    }
    protected override void OnInterect()
    {
        marketUI.gameObject.SetActive(true);
    }
}
