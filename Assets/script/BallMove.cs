using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BallMove : MonoBehaviour {
    [Header("运动参数")]
    public Vector3 velocity = new Vector3(0.001f, 0.001f, 0.001f);
    public float acceleration = 0f;
    public float maxSpeed = 100f;

    [Header("自动收集模式")]
    public bool autoCollectMode = true;

    private Rigidbody rb;
    private float radius;
    private float elapsedTime = 0f;
    public LayerMask collisionMask;

    public event Action<Vector3, Vector3, Vector3> OnHitBoard;

    void Awake() {
     //   collisionMask = LayerMask.GetMask("Default", "TopBoard", "DownBoard");
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("BallMove: 找不到 Rigidbody！");
        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;
    }

    void Start() {
        if (rb != null) {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    void FixedUpdate() {
        if (rb == null) return;

        float dt = Time.fixedDeltaTime;
        elapsedTime += dt;

        // 自动收集模式下：不做任何加速和归一化，直接使用当前速度向量
        if (!autoCollectMode) {
            float speed = velocity.magnitude;
            speed += acceleration * dt;
            speed = Mathf.Min(speed, maxSpeed);
            if (speed > 0.0001f)
                velocity = velocity.normalized * speed;
            else
                velocity = Vector3.zero;
        }

        // 处理移动和碰撞
        Vector3 moveDelta = velocity * dt;
        float dist = moveDelta.magnitude;
        if (dist < 0.0001f) return;

        Vector3 dir = moveDelta.normalized;
        RaycastHit hit;
        
        if (Physics.SphereCast(rb.position, radius, dir, out hit, dist, collisionMask)) {
            float safeDist = Mathf.Max(hit.distance - 0.002f, 0f);
            rb.MovePosition(rb.position + dir * safeDist);

            bool isTop = hit.collider.CompareTag("TopBoard");
            bool isDown = hit.collider.CompareTag("DownBoard");
            Debug.Log("Hit: " + hit.collider.name + " Tag: " + hit.collider.tag);
            if (isTop || isDown) {
                // 记录碰撞数据（位置、速度都是碰撞前的）
                OnHitBoard?.Invoke(hit.point, transform.position, velocity);

                
                    if (isTop && ScoreManager.Instance != null)
                        ScoreManager.Instance.AddScore_1(1);
                    else if (isDown && ScoreManager.Instance != null)
                        ScoreManager.Instance.AddScore_2(1);
                
            }

            // 反弹（只改变方向，大小不变）
            velocity = Vector3.Reflect(velocity, hit.normal);
        } else {
            rb.MovePosition(rb.position + moveDelta);
        }
    }

    public void ResetBall(Vector3 newPosition, Vector3 newVelocity) {
        if (rb == null) return;
        rb.position = newPosition;
        velocity = newVelocity;
        elapsedTime = 0f;
    }
}