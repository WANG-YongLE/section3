using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BallMove : MonoBehaviour {
    public Vector3 velocity = new Vector3(2f, 2f, 2f);
    public float maxSpeed = 5f;

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

        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;

        Vector3 dir = velocity.normalized;
        float dist = velocity.magnitude * dt;

        RaycastHit hit;

        if (Physics.SphereCast(rb.position, radius, dir, out hit, dist)) {

      
            float safeDist = Mathf.Max(hit.distance - 0.002f, 0f);
            rb.MovePosition(rb.position + dir * safeDist);

       
            velocity = Vector3.Reflect(velocity, hit.normal);

        } else {
            rb.MovePosition(rb.position + velocity * dt);
        }
    }
}