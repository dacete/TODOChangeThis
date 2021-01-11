using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
public class WorldManager : MonoBehaviour
{
    // Start is called before the first frame update
    public BasicNetManager netman;
    public PanelInOut loginPanel;
    public bool hasLoggedIn;
    public InputField usernameCreateField;
    public InputField passwordCreateField;
    public InputField usernameLoginField;
    public InputField passwordLoginField;
    public InputField temp;
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    bool listenForData; string serverIP;string serverPort;
    bool loggedIn;
    #endregion
    // Use this for initialization 	
    void Start()
    {
        serverIP = "178.79.150.7";
        ConnectToTcpServer();
        loginPanel.MovePanel(true);
    }
    public void FindMatchmakingServer()
    {
        if (loggedIn)
        {
            if (listenForData)
            {
                SendMessage("Matchmake");
            }
        }
    }
    public void CreateAccount()
    {
        CreateAccountContainer container = new CreateAccountContainer();
        string divider = "";
        container.username = usernameCreateField.text.ToString()+divider;
        print(passwordCreateField.text.ToString());
        container.password = ComputeSha256Hash(passwordCreateField.text.ToString()) + divider;
        container.authkey = "aw78df98ej90jf23er23" + divider;
        container.version = Application.version + divider;
        container.header = "CreateAccount" + divider;
        string json = JsonUtility.ToJson(container);
        print(json);
        SendMessage(json);
    }
    public void Login()
    {
        CreateAccountContainer container = new CreateAccountContainer();
        string divider = "";
        container.username = usernameLoginField.text.ToString() + divider;
        print(passwordCreateField.text.ToString());
        container.password = ComputeSha256Hash(passwordLoginField.text.ToString()) + divider;
        container.authkey = "aw78df98ej90jf23er23" + divider;
        container.version = Application.version + divider;
        container.header = "LoginUser" + divider;
        string json = JsonUtility.ToJson(container);
        print(json);
        SendMessage(json);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient(serverIP, 7777);
            Byte[] bytes = new Byte[1024];
            while (true && listenForData)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        JSONNode N = JSON.Parse(serverMessage);
                        Debug.Log("server message received as: " + serverMessage);
                        if(serverMessage == "LoginSuccessful")
                        {
                            loggedIn = true;
                        }
                        if (N["header"] != null)
                        {
                            if(N["header"] == "SubServerFound")
                            {
                                string ip = N["ip"];
                                string port = N["port"];
                                CloseConnection();
                                serverIP = ip;
                                serverPort = port;
                                ConnectToTcpServer();
                            }
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    private void SendMessage(string message)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {

                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    private void OnApplicationQuit()
    {
        if (socketConnection != null)
        {
            socketConnection.Close();
            listenForData = false;
        }
    }
    public void CloseConnection()
    {
        if (socketConnection != null)
        {
            socketConnection.Close();
            listenForData = false;
        }
    }
    static string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
[Serializable]
public class CreateAccountContainer
{
    public string username;
    public string password;
    public string authkey;
    public string version;
    public string header;
}