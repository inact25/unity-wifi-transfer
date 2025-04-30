using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class FileSender : MonoBehaviour
{
    public int port = 8888; // Port for the TCP server
    public string filePath = ""; // Path of the file to send
    public UIManager uiManager; // Reference to UIManager to update progress
    public Text ipText;
    
    private TcpListener server; // TCP server for listening to incoming connections
    private bool isServerRunning = false; // Whether the server is running or not
    
    void Start()
    {
        // Get the local IP address and update the IPText UI element
        string localIP = GetLocalIPAddress();
        ipText.text = "Server IP: " + localIP; // Display the IP on the UI

        // Set default value in IP input field (optional, if you want to show the local IP initially)
    }

    private void OnApplicationQuit()
    {
        StopServer(); // Stop the server when the application is closed
    }
    
    public string GetLocalIPAddress()
    {
        string localIP = string.Empty;

        // Get the host machine name
        string hostName = Dns.GetHostName();

        // Get the host entries associated with the machine name
        foreach (var ip in Dns.GetHostAddresses(hostName))
        {
            // Check for IPv4 addresses
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }

        return localIP;
    }

    public void StartServer()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is empty.");
            return;
        }
        StartCoroutine(ServerCoroutine()); // Start the server on a coroutine
    }

    IEnumerator ServerCoroutine()
    {
        server = new TcpListener(IPAddress.Any, port); // Listen on any available IP and specified port
        server.Start();
        isServerRunning = true;
        uiManager.UpdateStatus("Waiting for receiver...");

        while (isServerRunning)
        {
            if (server.Pending()) // Check if a client is attempting to connect
            {
                TcpClient client = server.AcceptTcpClient(); // Accept incoming client connection
                NetworkStream stream = client.GetStream(); // Create a network stream to send data

                byte[] fileData = File.ReadAllBytes(filePath); // Read the entire file into a byte array
                int fileSize = fileData.Length; // Get the file size

                // Send file size first (4 bytes)
                byte[] fileSizeBytes = System.BitConverter.GetBytes(fileSize);
                stream.Write(fileSizeBytes, 0, fileSizeBytes.Length);

                int bufferSize = 1024 * 4; // 4KB buffer for sending data
                int bytesSent = 0;

                // Send the file data in chunks
                while (bytesSent < fileSize)
                {
                    int bytesToSend = Mathf.Min(bufferSize, fileSize - bytesSent); // Send the remaining bytes
                    stream.Write(fileData, bytesSent, bytesToSend);
                    bytesSent += bytesToSend;

                    float progress = (float)bytesSent / fileSize; // Calculate the progress
                    uiManager.UpdateProgress(progress); // Update the UI with the progress
                }

                uiManager.UpdateStatus("File sent successfully!"); // Update UI with success message
                client.Close(); // Close the client connection
                StopServer(); // Stop the server after the file is sent
            }
            yield return null; // Wait for the next frame to check again
        }
    }

    // Stop the server
    public void StopServer()
    {
        if (server != null)
        {
            server.Stop();
            isServerRunning = false;
            Debug.Log("Server stopped.");
        }
    }
}
