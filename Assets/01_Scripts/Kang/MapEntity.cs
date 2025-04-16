using UnityEngine;

public abstract class MapEntity : MonoBehaviour
{
    public Sprite icon; // ������ ǥ�õ� ������
    public GameObject iconPrefab; // ������ ������ ������Ʈ
    GameObject iconObj;
    [HideInInspector] public SpriteRenderer sr;
    protected bool isMove = false;
    protected bool isRotate = false;

    private void CreateMapIcon()
    {
        iconObj = Instantiate(iconPrefab);
        if (!isMove)
            iconObj.transform.SetParent(transform);
        iconObj.transform.position = transform.position + Vector3.up * 400f; // ������ ��ġ ����

        sr = iconObj.GetComponent<SpriteRenderer>();
        sr.sprite = icon;
    }

    protected virtual void Start()
    {
        CreateMapIcon(); // ���� ���� �� ������ ����
    }
    protected virtual void Update()
    {
        if (iconObj.activeSelf == true)
        {
            if(isMove)
                iconObj.transform.position = transform.position + Vector3.up * 400f; // ������ ��ġ ����
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
