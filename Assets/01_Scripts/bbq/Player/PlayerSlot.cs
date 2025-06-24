using UnityEngine;

public class PlayerSlot : MonoBehaviour
{
    public IEquipable currentEquip;
    public Item currentBait;  // 현재 장착된 미끼
    public bool CanChange = true;
    public Transform handEquipPoint;
}
