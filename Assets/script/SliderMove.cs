using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using System;

public class MultiSliderController : MonoBehaviour {
    ROSConnection ros;

    public SliderConfig[] sliders;
    public float speed = 0.0001f;

    void Start() {
        ros = ROSConnection.GetOrCreateInstance();

        foreach (var slider in sliders) {
            // 初始化 current position
            if (slider.joint != null)
                slider.currentPosition = slider.joint.jointPosition[0];


            
            if (string.IsNullOrWhiteSpace(slider.topicName)) {
                Debug.Log("Topic name is empty! Skipping this slider.");

            }
            else {string topic = slider.topicName;
            ros.Subscribe<Float32Msg>(topic, msg => {
                OnReceive(slider, msg);
            });
            }



            if (slider.joint != null) {
                var drive = slider.joint.xDrive;
                drive.stiffness = 1000;
                drive.damping = 100;
                drive.forceLimit = 1000;
                slider.joint.xDrive = drive;
            }
        }
    }

    void OnReceive(SliderConfig slider, Float32Msg msg) {
        slider.targetPosition = Mathf.Clamp(msg.data, -0.12f, 0.12f);

        Debug.Log($"[{slider.topicName}] -> {msg.data}");
    }

    void Update() {
        foreach (var slider in sliders) {
            if (slider.joint == null) continue;

            slider.currentPosition = Mathf.MoveTowards(
                slider.currentPosition,
                slider.targetPosition,
                speed * Time.deltaTime
            );

            ApplyToJoint(slider);
        }
    }

    void ApplyToJoint(SliderConfig slider) {
        var drive = slider.joint.xDrive;
        drive.target = slider.currentPosition;
        slider.joint.xDrive = drive;
    }
    [System.Serializable]
    public class SliderConfig {
        public string topicName;           
        public ArticulationBody joint;     
        public float targetPosition = 0f;

        [HideInInspector]
        public float currentPosition = 0f;
    }
}