using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class JointStatePublisher : MonoBehaviour {
    public JointConfig[] joints;
    public float publishRate = 0.1f; // 每 0.1 秒發一次

    private ROSConnection ros;
    private float timer;

    void Start() {
        ros = ROSConnection.GetOrCreateInstance();

        foreach (var j in joints) {
            ros.RegisterPublisher<Float32Msg>(j.topicName);
        }
    }

    void Update() {
        timer += Time.deltaTime;

        if (timer >= publishRate) {
            timer = 0f;

            foreach (var j in joints) {
                float position = (float)j.joint.jointPosition[0];
                ros.Publish(j.topicName, new Float32Msg(position));
            }
        }
    }

    [System.Serializable]
    public class JointConfig {
        public string topicName;
        public ArticulationBody joint;
    }
}