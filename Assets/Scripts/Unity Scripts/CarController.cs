using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheelCollider;
    public WheelCollider rightWheelCollider;
    public Transform leftWheelTransform;
    public Transform rightWheelTransform;
    public bool motor;
    public bool steering;
}

public class CarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    public RecieveSteering recieveSteering;
    public bool userControl;  

    private void UpdateWheelVisuals(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);

        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    void Update()
    {
        if (recieveSteering.connected || userControl)
        {
            float currentMotorSpeed = maxMotorTorque * 1; //from -100 to 100
            float input = userControl ? Input.GetAxis("Horizontal") : recieveSteering.prediction;
            float currentSteering;

            if (input > 0)
            {
                currentSteering = maxSteeringAngle * 1;
            }
            else if (input < 0)
            {
                currentSteering = maxSteeringAngle * -1;
            }
            else
            {
                currentSteering = maxSteeringAngle * 0;
            }

            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheelCollider.steerAngle = currentSteering;
                    axleInfo.rightWheelCollider.steerAngle = currentSteering;
                }

                if (axleInfo.motor)
                {
                    axleInfo.leftWheelCollider.motorTorque = currentMotorSpeed;
                    axleInfo.rightWheelCollider.motorTorque = currentMotorSpeed;
                }

                UpdateWheelVisuals(axleInfo.leftWheelCollider, axleInfo.leftWheelTransform);
                UpdateWheelVisuals(axleInfo.rightWheelCollider, axleInfo.rightWheelTransform);
            }
        }
        
    }
}
