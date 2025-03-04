using UnityEngine;

public abstract class MapEntity : MonoBehaviour
{
    public Sprite icon; // 지도에 표시될 아이콘
    public GameObject iconPrefab; // 생성된 아이콘 오브젝트
    GameObject iconObj;
    protected bool isDynamic = false;

    private void CreateMapIcon()
    {
        iconObj = Instantiate(iconPrefab);
        if (!isDynamic)
            iconObj.transform.SetParent(transform);
        iconObj.transform.position = transform.position + Vector3.up * 400f; // 아이콘 위치 조정

        SpriteRenderer sr = iconObj.GetComponent<SpriteRenderer>();
        sr.sprite = icon;
    }

    protected virtual void Start()
    {
        CreateMapIcon(); // 게임 시작 시 아이콘 생성
    }
    private void Update()
    {
        if (isDynamic)
        {
            iconObj.transform.position = transform.position + Vector3.up * 400f; // 아이콘 위치 조정
        }
    }
    public void IconEnable()
    {
        iconObj.SetActive(true);
    }
    public void IconDisable()
    {
        iconObj.SetActive(false);
    }
}
