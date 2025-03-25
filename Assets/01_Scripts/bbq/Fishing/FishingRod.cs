using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

[CreateAssetMenu(fileName = "FishingRod", menuName = "Items/FishingRod")]
public class FishingRod : Item, IEquipable
{
    [SerializeField] private Fishing fisherBase;
    [SerializeField] private FishCanvas fishCanvasBase;
    [SerializeField] private FishingVisual fishingVisualBase;

    private Fishing fisher;
    private FishingVisual fishingVisual;
    private FishCanvas fishCanvas;

    private Player owner;

    public void Equip(Player player)
    {
        owner = player;
        owner.playerSlot.currentEquip = this;
        var hand = owner.playerSlot.handEquipPoint;
        fishingVisual = Instantiate(fishingVisualBase, hand);
        fisher = Instantiate(fisherBase);   
        fisher.transform.SetParent(owner.transform,false);
        fisher.transform.localPosition = Vector3.zero + Vector3.up * 1f;
        fishCanvas = Instantiate(fishCanvasBase);
        fisher.fishCanvas = fishCanvas;
        fisher.fishingVisual = fishingVisual;
        fishingVisual.transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    public void Unequip()
    {
        TryDestroy(fisher);
        TryDestroy(fishCanvas);  
        TryDestroy(fishingVisual);
    }

    public void TryDestroy(Object obj)
    {
        try{
            if (obj == null || obj.IsDestroyed()) return;
            Destroy(obj.GameObject());
            // obj = null;
        }   
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    void OnDestroy()
    {
        Unequip();
    }
}








