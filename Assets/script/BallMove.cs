using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BallMove : MonoBehaviour {
    public Vector3 velocity = new Vector3(0.001f, 0.001f, 0.001f);
    public float acceleration = 0.04f;
    public float maxSpeed;   //  改成會變動的
    [SerializeField] private string topicVX = "/ball_vx";
    [SerializeField] private string topicVY = "/ball_vy";
    [SerializeField] private string topicVZ = "/ball_vz";
    [SerializeField] private float publishInterval = 0.1f;
    Rigidbody rb;
    float radius;
    private ROSConnection ros;
    private float publishTimer = 0f;
    float elapsedTime = 0f;  // 用來記時間
    public Vector3 Velocity => velocity;
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;

        maxSpeed = GetMaxSpeedByTime(0f);
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32Msg>(topicVX);
        ros.RegisterPublisher<Float32Msg>(topicVY);
        ros.RegisterPublisher<Float32Msg>(topicVZ);
    }

    void FixedUpdate() {
        float dt = Time.fixedDeltaTime;

        // 累積時間
        elapsedTime += dt;

        // 更新 maxSpeed
        maxSpeed = GetMaxSpeedByTime(elapsedTime);

        float speed = velocity.magnitude;
        speed += acceleration * dt;
        speed = Mathf.Min(speed, maxSpeed);

        velocity = velocity.normalized * speed;

        Vector3 dir = velocity.normalized;
        float dist = velocity.magnitude * dt;

        RaycastHit hit;

        if (Physics.SphereCast(rb.position, radius, dir, out hit, dist)) {
            float safeDist = Mathf.Max(hit.distance - 0.002f, 0f);
            rb.MovePosition(rb.position + dir * safeDist);

            if (hit.collider.CompareTag("TopBoard"))
                ScoreManager.Instance.AddScore_1(1);
            else if (hit.collider.CompareTag("DownBoard"))
                ScoreManager.Instance.AddScore_2(1);
            //else if (!hit.collider.CompareTag("Wall")) {
            // Time.timeScale = 0f;
            //  }
            velocity = Vector3.Reflect(velocity, hit.normal);
        } else {
            rb.MovePosition(rb.position + velocity * dt);
        }
        publishTimer += dt;
        if (publishTimer < publishInterval) return;
        publishTimer = 0f;

        PublishSpeed();
    }
    private void PublishSpeed() {
        ros.Publish(topicVX, new Float32Msg(velocity.x));
        ros.Publish(topicVY, new Float32Msg(velocity.y));
        ros.Publish(topicVZ, new Float32Msg(velocity.z));
    }
    float GetMaxSpeedByTime(float t) {
        if (t < 10f) return 0.01f;
        else if (t < 20f) return 0.1f;
        else if (t < 30f) return 0.4f;
        else if (t < 40f) return 1f;
        else if (t < 50f) return 2f;
        else return 5f;
    }
}