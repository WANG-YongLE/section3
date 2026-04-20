using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BallMove : MonoBehaviour {
    public Vector3 velocity = new Vector3(0.001f, 0.001f, 0.001f); 
    public float maxSpeed = 5f; 
    public float acceleration = 0.04f; 
    Rigidbody rb;
    float radius;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;
    }

    void FixedUpdate() {
        float dt = Time.fixedDeltaTime;

      
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

            //處理碰撞
            if (hit.collider.CompareTag("TopBoard")) {
                ScoreManager.Instance.AddScore_1(1);
            } else if (hit.collider.CompareTag("DownBoard")) {
                ScoreManager.Instance.AddScore_2(1);
            } 
            //else if (!hit.collider.CompareTag("Wall")) {
               // Time.timeScale = 0f;
          //  }

            //  反射
            velocity = Vector3.Reflect(velocity, hit.normal);

        } else {
            rb.MovePosition(rb.position + velocity * dt);
        
        }
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