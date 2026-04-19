using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class UnityPublisher : MonoBehaviour {
    ROSConnection ros;
    public string topicName = "unity_topic";

    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<StringMsg>(topicName);

        InvokeRepeating("PublishMessage", 1.0f, 2.0f);
    }

    void PublishMessage() {
        StringMsg msg = new StringMsg("Hello from Unity");
        ros.Publish(topicName, msg);
        Debug.Log("Sent message");
    }
}