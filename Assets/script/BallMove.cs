using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallMove : MonoBehaviour {
    Vector3 velocity = new Vector3(5f, 5f, 0f);
    Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true; 
    }

    void FixedUpdate() {
        float dt = Time.fixedDeltaTime;
        Vector3 move = velocity * dt;

        RaycastHit hit;
        if (Physics.Raycast(rb.position, move.normalized, out hit, move.magnitude)) {
           
            Vector3 normal = hit.normal;

           
            velocity = Vector3.Reflect(velocity, normal);

            rb.position = hit.point + normal * 0.001f;
        } else {
  
            rb.MovePosition(rb.position + move);
        }
    }
}