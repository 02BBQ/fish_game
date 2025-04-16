using UnityEngine;

public abstract class MapEntity : MonoBehaviour
{
    public Sprite icon; // 지도에 표시될 아이콘
    public GameObject iconPrefab; // 생성된 아이콘 오브젝트
    GameObject iconObj;
    [HideInInspector] public SpriteRenderer sr;
    protected bool isMove = false;
    protected bool isRotate = false;

    private void CreateMapIcon()
    {
        iconObj = Instantiate(iconPrefab);
        if (!isMove)
            iconObj.transform.SetParent(transform);
        iconObj.transform.position = transform.position + Vector3.up * 400f; // 아이콘 위치 조정

        sr = iconObj.GetComponent<SpriteRenderer>();
        sr.sprite = icon;
    }

    protected virtual void Start()
    {
        CreateMapIcon(); // 게임 시작 시 아이콘 생성
    }
    protected virtual void Update()
    {
        if (iconObj.activeSelf == true)
        {
            if(isMove)
                iconObj.transform.position = transform.position + Vector3.up * 400f; // 아이콘 위치 조정
            if (isRotate)
                iconObj.transform.rotation = Quaternion.Euler(90f, transform.GetChild(0).eulerAngles.y, 0f);
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
