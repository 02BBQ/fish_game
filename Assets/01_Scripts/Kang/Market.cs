using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour
{
    public List<Item> items;
    public Transform shopUIRoot;
    public Transform shopUIParent;
    public GameObject goods;
    void Start()
    {
        foreach(Item item in items)
        {
            Goods copyGoods = Instantiate(goods, shopUIParent).GetComponent<Goods>();
            copyGoods.SetItem(item);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopUIRoot.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopUIRoot.gameObject.SetActive(false);
        }
    }
}
