using UnityEngine;

public abstract class MapEntity : MonoBehaviour
{
    public Sprite icon; // ������ ǥ�õ� ������
    public GameObject iconPrefab; // ������ ������ ������Ʈ
    GameObject iconObj;
    protected bool isDynamic = false;

    private void CreateMapIcon()
    {
        iconObj = Instantiate(iconPrefab);
        if (!isDynamic)
            iconObj.transform.SetParent(transform);
        iconObj.transform.position = transform.position + Vector3.up * 400f; // ������ ��ġ ����

        SpriteRenderer sr = iconObj.GetComponent<SpriteRenderer>();
        sr.sprite = icon;
    }

    protected virtual void Start()
    {
        CreateMapIcon(); // ���� ���� �� ������ ����
    }
    private void Update()
    {
        if (isDynamic)
        {
            iconObj.transform.position = transform.position + Vector3.up * 400f; // ������ ��ġ ����
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
