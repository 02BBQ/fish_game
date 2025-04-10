using System.Collections.Generic;
using UnityEngine;

public class BoatManager : SingleTon<BoatManager>
{
    public List<Item> boatItems;
    public List<BoatController> boats = new();

    private void Start()
    {
        for(int i = 1; i < boats.Count; i++)
        {
            boats[i].gameObject.SetActive(false);
        }
        UnlockBoat(boatItems[0]);
    }
    public void UnlockBoat(Item item)
    {
        BoatController boat = boats[boatItems.IndexOf(item)];
        boat.gameObject.SetActive(false);
        UIManager.Instance.MakeBoatLabel(item, boat);
    }
}
