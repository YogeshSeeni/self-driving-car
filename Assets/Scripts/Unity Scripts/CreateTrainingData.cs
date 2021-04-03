using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CreateTrainingData : MonoBehaviour
{

    public GameObject RayCastObject;

    private float speed;
    private Vector3 oldPosition;

    RaycastHit leftHit;
    RaycastHit rightHit;
    RaycastHit frontRight;
    RaycastHit frontLeft;
    RaycastHit forwardHit;

    private string csvPath = "asdf";
    private StringBuilder csvContent = new StringBuilder();

    void Start()
    {
        csvPath = Application.dataPath + "/training_data.csv";
        csvContent.AppendLine("ForwardRay,ForwardLeftRay,LeftRay,ForwardRightRay,RightRay,Speed,SteeringAngle");

        oldPosition = transform.position;    
    }

    private void FixedUpdate()
    {
        forwardHit = UpdateRayCast(forwardHit, RayCastObject.transform.forward);
        rightHit = UpdateRayCast(rightHit, RayCastObject.transform.right);
        leftHit = UpdateRayCast(leftHit, -RayCastObject.transform.right);
        frontRight = UpdateRayCast(frontRight, RayCastObject.transform.right + RayCastObject.transform.forward);
        frontLeft = UpdateRayCast(frontLeft, -RayCastObject.transform.right + RayCastObject.transform.forward);
        UpdateSpeed();
    }

    private RaycastHit UpdateRayCast(RaycastHit hit, Vector3 direction)
    {
        if (Physics.Raycast(RayCastObject.transform.position, direction, out hit))
        {
            Debug.DrawRay(RayCastObject.transform.position, direction * hit.distance, Color.green);
        }
        return hit;
    }

    private void UpdateSpeed()
    {
        speed = Vector3.Distance(oldPosition, transform.position) * 100f;
        oldPosition = transform.position;
    }

    private void Update()
    {
        int currentInput;

        if (Input.GetAxis("Horizontal") > 0)
        {
            currentInput = 1;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            currentInput = -1;
        }
        else
        {
            currentInput = 0;
        }

        csvContent.AppendLine(forwardHit.distance.ToString() + "," + frontLeft.distance.ToString() + "," + leftHit.distance.ToString() + "," + frontRight.distance.ToString() + "," + rightHit.distance.ToString() + "," + currentInput.ToString());
    }

    private void OnApplicationQuit()
    {
        File.AppendAllText(csvPath, csvContent.ToString());
    }
}
