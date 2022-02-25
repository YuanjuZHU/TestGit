using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;


public class UDP_Connection : MonoBehaviour
{
    private UdpClient udpServer;
    private Thread t;
    private static IPEndPoint remoteEP_Send;
    private static IPEndPoint remoteEP_Send2;
    private IPEndPoint remoteEP_Receive;




    void Start() {

        remoteEP_Send = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 26000); //Local host: 127.0.0.1
        remoteEP_Send2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27000); //Local host: 127.0.0.1

        remoteEP_Receive = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 26001);

        udpServer = new UdpClient(26001);
        t = new Thread(() => {
            while(true) {
                this.receiveData();
            }
        });
        t.Start();
        t.IsBackground = true;


        //senderUdpClient();
    }



    void Update() {
        //Debug.Log("Start");
        //if(Input.GetKeyDown(KeyCode.Return))
        //{
        //    senderUdpClient();
        //}
        
    }

    private void OnApplicationQuit() {
        udpServer.Close();
        t.Abort();
    }

    private void receiveData()
    {
        byte[] data = udpServer.Receive(ref remoteEP_Receive);

        var text = "";
        foreach(var b in data) {
            text = text + " " + b;

        }
        Debug.Log("Received data byte dimension: " + data.Length);
        Debug.Log("Received data byte: " + text);

        if(data.Length > 0)
        {
            var str = Encoding.UTF8.GetString(data);
            Debug.Log("Received data: " + str);
        }
    }

    public static void senderUdpClient(string jsonFile)
    {
        UdpClient senderClient = new UdpClient();
        senderClient.Connect(remoteEP_Send);

        UdpClient senderClient2 = new UdpClient();
        senderClient2.Connect(remoteEP_Send2);

        byte[] bytes = toBytes(jsonFile);
        byte[] bytes2 = toBytes(bytes.Length.ToString());

        Debug.Log("Dati inviati " + bytes.Length);
        var text = "";
        foreach (var b in bytes)
        {
            text = text + " " + b;

        }
        Debug.Log(text);

        senderClient.Send(bytes, bytes.Length);
        senderClient2.Send(bytes2, bytes2.Length);


        //string sendString = "0.4";
        //Debug.Log("Sent data " + sendString);
        //byte[] bytes = toBytes(sendString);

        //Thread t = new Thread(() => {
        //    while(true) {
        //        Debug.Log("Dati inviati");
        //        senderClient.Send(bytes, bytes.Length);
        //        Thread.Sleep(1000);
        //    }
        //});
        //t.Start();
    }

    static byte[] toBytes(string text) {
            return Encoding.UTF8.GetBytes(text);
        }
    }
