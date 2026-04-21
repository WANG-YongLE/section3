using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class BallPublisher : MonoBehaviour {
    ROSConnection ros;

    public string topicName = "/ball_position";
    public GameObject ball;

    public float publishRate = 0.05f; // 20Hz
    private float timer = 0f;

    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PointMsg>(topicName);
    }

    void Update() {
        timer += Time.deltaTime;

        if (timer < publishRate) return;
        timer = 0f;

        PublishBallPosition();
    }

    void PublishBallPosition() {
        Vector3 pos = ball.transform.position;

        PointMsg msg = new PointMsg {
            x = pos.x,
            y = pos.y,
            z = pos.z
        };

        ros.Publish(topicName, msg);

        Debug.Log($"Ball Pos: {pos}");
    }
}