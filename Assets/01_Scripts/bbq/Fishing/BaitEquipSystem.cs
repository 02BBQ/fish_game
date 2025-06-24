using UnityEngine;
using UnityEngine.UI;

namespace bbq.Fishing
{
    public class BaitEquipSystem : MonoBehaviour
    {
        private static BaitEquipSystem _instance;
        public static BaitEquipSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BaitEquipSystem>();
                }
                return _instance;
            }
        }

        [SerializeField] private GameObject equippedBaitIcon;  // 장착된 미끼 아이콘을 표시할 UI 오브젝트
        [SerializeField] private Image equippedBaitImage;      // 미끼 이미지를 표시할 Image 컴포넌트

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void EquipBait(Item bait)
        {
            if (bait.type != ItemType.Bait)
            {
                Debug.LogWarning("Attempted to equip non-bait item as bait");
                return;
            }

            var playerSlot = Definder.Player.playerSlot;
            playerSlot.currentBait = bait;
            
            // UI 업데이트
            if (equippedBaitIcon != null)
            {
                equippedBaitIcon.SetActive(true);
                if (equippedBaitImage != null)
                {
                    equippedBaitImage.sprite = bait.image;
                }
            }
        }

        public void UnequipBait()
        {
            var playerSlot = Definder.Player.playerSlot;
            playerSlot.currentBait = null;
            
            // UI 숨기기
            if (equippedBaitIcon != null)
            {
                equippedBaitIcon.SetActive(false);
            }
        }

        public Item GetEquippedBait()
        {
            return Definder.Player.playerSlot.currentBait;
        }
    }
} 