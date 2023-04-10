using UnityEngine;

public class TesterController : MonoBehaviour
{
    public float rayDistance = 10f;
    public float slopeThreshold = 45f;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        Vector2 targetVelocity = Vector2.zero;

        targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * transform.right.x * 10f, 0f);
        rb.velocity = targetVelocity;
        // 레이캐스트 시작점
        Vector2 rayOrigin = transform.position;
        // 레이캐스트 방향
        Vector2 rayDirection = transform.right;
        // 레이캐스트 결과
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, LayerMask.GetMask("Ground"));
        print(rayDirection);
        if (hit.collider != null)
        {
            // 충돌한 객체의 경사면 각도 계산
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            print("경사각은 " + slopeAngle + "도 입니다.");
            // 경사면 각도가 지정한 경사각 이하일 경우
            /*if (slopeAngle <= slopeThreshold)
            {
                Debug.Log("45도 경사면이 있습니다.");
            }
            else
            {
                Debug.Log("45도 경사면이 없습니다.");
            }*/
            Debug.DrawRay(hit.point, hit.normal, Color.red);
        }
        else
        {
            Debug.Log("레이캐스트가 충돌하지 않았습니다.");
        }
    }
}
