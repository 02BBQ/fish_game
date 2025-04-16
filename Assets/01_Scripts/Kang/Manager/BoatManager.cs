using System.Collections.Generic;
using UnityEngine;

public class BoatManager : SingleTon<BoatManager>
{
    public List<Item> boatItems;
    public List<BoatController> boats = new();
    public List<BoatLabel> labels;
    public Dock mainDock;
    Dock currentDock;
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
    public void SetDock(Dock dock)
    {
        currentDock = dock;
    }
    public void UnlockBoat(Item item)
    {
        BoatController boat = boats[boatItems.IndexOf(item)];
        boat.gameObject.SetActive(true);
        UIManager.Instance.MakeBoatLabel(item, boat);
    }
    public void Sort()
    {
        currentDock = mainDock;
        DockSort();
    }

    public void DockSort()
    {
        int index = 0;
        print(labels.Count);
        print(currentDock.points.Count);

        foreach (BoatLabel label in labels)
        {
            print(label.boat.gameObject.name);
            if (label.boatActive)
            {
                if (index >= currentDock.points.Count)
                    label.OnClickToggle();
                else
                {
                    label.boat.ResetPos(currentDock.points[index]);
                    index++;
                }
            }
        }
    }
}
