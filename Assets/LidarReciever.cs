﻿using UnityEngine;
using System; //Needed for Exceptions
using System.Collections;
using System.Threading; //Used for Thread
using System.Net; //Used for IPEndPoint
using System.Net.Sockets; //Used for UDPClient
using System.Text; //Used for Encoding

public class LidarReciever : MonoBehaviour {

    private GameObject[] pointObjects = new GameObject[370];
    public GameObject pointObjectPrefab;

    private UdpClient client;
    private  IPEndPoint remoteEndPoint;

    public int port = 8051;
    public string ip = "127.0.0.1";

    public string data = " ";
    public string requestString = " ";
    private byte[] requestBytes;

    public bool okToRecieve = false;

    // Use this for initialization
    void Start () {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        client = new UdpClient();
        client.Connect(remoteEndPoint);
        okToRecieve = true;

         requestBytes = Encoding.UTF8.GetBytes(requestString);
    }
	
	// Update is called once per frame
	void Update () {
        if (okToRecieve)
        {
            okToRecieve = false;
            client.Send(requestBytes, requestBytes.Length);
            StartCoroutine(WaitForData());

        }
        
	}
    IEnumerator WaitForData() {

        data = client.Receive(ref remoteEndPoint).ToString();
        yield return StartCoroutine(ParseData(data));
    }
    IEnumerator ParseData(string data) {
        Queue lidarQueue = new Queue();
        LidarPoint newPoint;

        string[] dataSplit = data.Split('|');
        string metaData = dataSplit[0];
        Console.Write("Meta Data: " + metaData);

        string[] dataBunch = dataSplit[0].Split(';');

        string[] dataPair;
        int i = 0;
        foreach (string s in dataBunch)
        {
            dataPair = s.Split(',');
            newPoint = new LidarPoint(i + " :: " + metaData, dataPair[0], dataPair[1]);
            lidarQueue.Enqueue(newPoint);
            i++;
        }
        yield return StartCoroutine(DrawPoints(lidarQueue));
    }
    IEnumerator DrawPoints(Queue lidarQueue) {

        okToRecieve = true;
        yield return 0;
    }

}