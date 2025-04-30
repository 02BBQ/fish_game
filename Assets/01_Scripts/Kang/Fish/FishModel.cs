using UnityEngine;

public class FishModel : MonoBehaviour
{
    public float speed = 2.0f; // Fish의 이동 속도
    public float turnSpeed = 2.0f; // 방향 전환 속도
    public SpriteRenderer spriteRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRend;
    
    private Vector3 direction; // Fish의 이동 방향
    private BoxCollider boxCollider; // Fish가 있는 Box Collider
    private Quaternion targetRotation; // 목표 회전
    private bool isChangingDirection = false; // 방향 변경 여부
    private bool isMoving = true; // 이동 여부
    private bool triggerEntered = false; // Trigger Enter 여부


    public void Init(Mesh mesh, Material mat)
    {
        meshFilter.mesh = mesh;
        meshRend.material = mat;
        spriteRenderer.gameObject.SetActive(false);
        meshRend.gameObject.SetActive(true);
    }
    public void Init(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;

        spriteRenderer.gameObject.SetActive(true);
        meshRend.gameObject.SetActive(false);
    }

    void Start()
    {
        boxCollider = transform.parent.GetComponent<BoxCollider>(); // Box Collider 가져오기
        SetRandomDirection(); // 랜덤한 방향 설정
    }

    // Update is called once per frame
    void Update()
    {
        MoveFish(); // Fish 이동
    }

    void MoveFish()
    {
        if (isMoving)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        // 목표 회전으로 회전
        if (isChangingDirection)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                isChangingDirection = false; // 회전 완료
                if (triggerEntered)
                {
                    isMoving = true; // Trigger가 Enter된 경우 이동 시작
                }
                else
                {
                    // Trigger가 Enter되지 않은 경우 다시 회전
                    SetNewDirection();
                }
            }
        }
    }

    void SetRandomDirection()
    {
        // Forward 방향으로 설정
        direction = transform.forward; // Forward 방향으로 설정
    }

    void SetNewDirection()
    {
        // Z축 회전 설정
        float heightPercentage = boxCollider.bounds.min.y + boxCollider.bounds.size.y * 0.35f;
        float xRotation = heightPercentage < transform.position.y ? Random.Range(-30f, 0f) : Random.Range(0f, 30f);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, xRotation);

        // 새로운 방향을 랜덤하게 설정하되, y축은 0으로 고정
        float angle = Random.Range(90f, 270f) + transform.eulerAngles.y; // 90도에서 180도 사이의 각도
        targetRotation = Quaternion.Euler(xRotation, angle, 0f); // 목표 회전 설정
        isChangingDirection = true; // 방향 변경 상태로 설정
        isMoving = false; // 이동 중지
    }

    private void OnTriggerEnter(Collider other)
    {
        // Trigger가 Enter할 때
        if (other.transform == transform.parent)
        {
            triggerEntered = true; // Trigger Enter 상태 설정
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Trigger가 Exit할 때
        if (other.transform == transform.parent)
        {
            SetNewDirection();
            isMoving = false;
            triggerEntered = false; // Trigger Exit 상태 설정
        }
    }
}
