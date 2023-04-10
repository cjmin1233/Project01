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
        // ����ĳ��Ʈ ������
        Vector2 rayOrigin = transform.position;
        // ����ĳ��Ʈ ����
        Vector2 rayDirection = transform.right;
        // ����ĳ��Ʈ ���
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, LayerMask.GetMask("Ground"));
        print(rayDirection);
        if (hit.collider != null)
        {
            // �浹�� ��ü�� ���� ���� ���
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            print("��簢�� " + slopeAngle + "�� �Դϴ�.");
            // ���� ������ ������ ��簢 ������ ���
            /*if (slopeAngle <= slopeThreshold)
            {
                Debug.Log("45�� ������ �ֽ��ϴ�.");
            }
            else
            {
                Debug.Log("45�� ������ �����ϴ�.");
            }*/
            Debug.DrawRay(hit.point, hit.normal, Color.red);
        }
        else
        {
            Debug.Log("����ĳ��Ʈ�� �浹���� �ʾҽ��ϴ�.");
        }
    }
}
