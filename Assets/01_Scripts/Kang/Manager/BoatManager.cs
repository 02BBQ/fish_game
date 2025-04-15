using System.Collections.Generic;
using UnityEngine;

public class BoatManager : SingleTon<BoatManager>
{
    public List<Item> boatItems;
    public List<BoatController> boats = new();
    public List<BoatLabel> labels;
    public List<Transform> points;
    
    private void Start()
    {
        for(int i = 1; i < boats.Count; i++)
        {
            boats[i].gameObject.SetActive(false);
        }
        UnlockBoat(boatItems[0]);
    }
    private void OnEnable()
    {
        EventBus.Subscribe(EventBusType.Drowning, Sort);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(EventBusType.Drowning, Sort);
    }
    public void UnlockBoat(Item item)
    {
        BoatController boat = boats[boatItems.IndexOf(item)];
        boat.gameObject.SetActive(true);
        UIManager.Instance.MakeBoatLabel(item, boat);
    }
    public void Sort()
    {
        int index = 0;
        foreach (BoatLabel label in labels)
        {
            if (label.boatActive)
            {
                if (index >= points.Count)
                    label.OnClickToggle();
                else
                {
                    label.boat.transform.position = points[index].position;
                    index++;
                } 
            }
        }
    }
}
