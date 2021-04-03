using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.UI;

public class RecieveSteering : MonoBehaviour
{
    Thread mThread;
    private string IP;
    private int port;
    private IPAddress localAdd;
    private TcpListener listener;
    private TcpClient client;

    bool running;
    public bool connected = false;

    public GameObject RayCastObject;

    private float speed;
    private Vector3 oldPosition;

    public RaycastHit leftHit;
    public RaycastHit rightHit;
    public RaycastHit frontRight;
    public RaycastHit frontLeft;
    public RaycastHit forwardHit;

    public int prediction = 0;

    public Text speedText;
    public Text leftDistText;
    public Text leftFrontDistText;
    public Text frontDistText;
    public Text frontRightDistText;
    public Text rightDistText;
    public Text predictionText;

    public Text selectSettingsError;
    public Text waitingForConnection;

    
    private void Start()
    {
        if (PlayerPrefs.GetString("IP", "none") == "none" || PlayerPrefs.GetInt("Port", 0) == 0)
        {
            selectSettingsError.enabled = true;
        } else
        {
            IP = PlayerPrefs.GetString("IP");
            port = PlayerPrefs.GetInt("Port");
        }

        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }
    //Update speed and prediction and enable them
    private void Update()
    {
        if (connected)
        {
            waitingForConnection.enabled = false;
            speedText.enabled = true;
            frontDistText.enabled = true;
            rightDistText.enabled = true;
            leftDistText.enabled = true;
            frontRightDistText.enabled = true;
            leftFrontDistText.enabled = true;
            predictionText.enabled = true;
        }

        speedText.text = "Speed: " + speed.ToString();

        if (prediction == -1)
        {
            predictionText.text = "Prediction: Left";
        } else if (prediction == 0)
        {
            predictionText.text = "Prediction: Straight";
        } else
        {
            predictionText.text = "Prediction: Right";
        }
    }

    private void FixedUpdate()
    {
        forwardHit = UpdateRayCast(forwardHit, RayCastObject.transform.forward, "ForwardDist: ", frontDistText);
        rightHit = UpdateRayCast(rightHit, RayCastObject.transform.right, "RightDist: ", rightDistText);
        leftHit = UpdateRayCast(leftHit, -RayCastObject.transform.right, "LeftDist: ", leftDistText);
        frontRight = UpdateRayCast(frontRight, RayCastObject.transform.right + RayCastObject.transform.forward, "RightFrontDist: ", frontRightDistText);
        frontLeft = UpdateRayCast(frontLeft, -RayCastObject.transform.right + RayCastObject.transform.forward, "LeftFrontDist: ", leftFrontDistText);
        UpdateSpeed();
    }

    private RaycastHit UpdateRayCast(RaycastHit hit, Vector3 direction, String startText, Text text)
    {
        if (Physics.Raycast(RayCastObject.transform.position, direction, out hit))
        {
            text.text = startText + hit.distance.ToString();
        }
        return hit;
    }

    private void UpdateSpeed()
    {
        speed = Vector3.Distance(oldPosition, transform.position) * 100f;
        oldPosition = transform.position;
    }

    private void GetInfo()
    {
        localAdd = IPAddress.Parse(IP);
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        client = listener.AcceptTcpClient();
        connected = true;

        running = true;
        while (running)
        {
            SendAndRecieve();
        }
        listener.Stop();
    }

    private void SendAndRecieve()
    {
        

        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        byte[] writeBuffer = Encoding.ASCII.GetBytes(forwardHit.distance.ToString() + "," + frontLeft.distance.ToString() + "," + leftHit.distance.ToString() + "," + frontRight.distance.ToString() + "," + rightHit.distance.ToString());
        nwStream.Write(writeBuffer, 0, writeBuffer.Length);

        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        string dataRecieved = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        if (dataRecieved != null)
        {
            prediction = Int32.Parse(dataRecieved) -1;
            //print("Recieved: " + prediction);
        }
    }
}
