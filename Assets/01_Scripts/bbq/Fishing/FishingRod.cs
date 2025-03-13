using UnityEngine;

[CreateAssetMenu(fileName = "FishingRod", menuName = "Items/FishingRod")]
public class FishingRod : Item, IEquipable
{
    [SerializeField] private Fishing fisherBase;
    [SerializeField] private FishCanvas fishCanvasBase;

    private Fishing fisher;
    private FishCanvas fishCanvas;

    private Player owner;

    public void Equip(Player player)
    {
        owner = player;
        owner.playerSlot.currentEquip = this;
        fisher = Instantiate(fisherBase);   
        fisher.transform.SetParent(owner.transform);
        fishCanvas = Instantiate(fishCanvasBase);
    }

    public void Unequip()
    {
        Destroy(fisher);
        Destroy(fishCanvas);  
    }
}








