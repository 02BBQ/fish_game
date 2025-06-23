using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Market : Interactor
{
    public List<Item> buyItems;
    public List<Item> sellItems;
    public Transform marketUI;
    public Transform[] buyUIParents;
    public Transform[] sellUIParents;
    public GameObject buyGoods;
    public GameObject sellGoods;

    private List<SellGoods> initItems;

    private void Awake()
    {
        LoadAndSortItems();    
        initItems = new List<SellGoods>();
    }

    public void LoadAndSortItems()
    {
        Addressables.LoadAssetsAsync<Item>("Items", item =>
        {
            buyItems.Add(item);
        }).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 가격순 정렬 (오름차순)
                buyItems = buyItems.OrderBy(x => x.price).ToList(); 

                foreach (Item item in buyItems)
                {
                    AddGoodsInBuy(item);
                }
            }
        };
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        EventManager.AddListener<ItemAddedEvent>(AddGoodInSell);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<ItemAddedEvent>(AddGoodInSell);
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

    private void AddGoodInSell(ItemAddedEvent addItemEvent)
    {
        Item addItem = addItemEvent.newItem;
        bool isItemInInit = false; //initItems.Any(initItem => initItem.item.nameStr == addItem.nameStr);
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
    protected override void OnInteract()
    {
        marketUI.gameObject.SetActive(true);
    }
}
