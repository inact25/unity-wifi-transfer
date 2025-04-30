using System.Collections;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI; // Make sure to import this for Text and InputField

public class FileReceiver : MonoBehaviour
{
    public string savePath; // Where the file will be saved
    public string serverIP; // Default server IP
    public int port = 8888; // Port of the server
    public UIManager uiManager; // Reference to UIManager to update progress
    public Text ipText; // Reference to the Text component for displaying IP
    public InputField ipInputField; // Reference to the InputField for entering the IP address

    private TcpClient client; // Client to connect to the server

    void Start()
    {
        // Get the local IP address and update the IPText UI element
        string localIP = GetLocalIPAddress();
        ipText.text = "Receiver IP: " + localIP; // Display the IP on the UI

        // Set default value in IP input field (optional, if you want to show the local IP initially)
        ipInputField.text = "127.0.0.1";

        // Set the save path to persistent data path
        savePath = Path.Combine(Application.persistentDataPath, "received_file"); // Save in the persistent data path

        // Optional: Log the save path for testing
        Debug.Log("File will be saved to: " + savePath);
    }

    // Method to get the local IP address of the device
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

    public void ConnectToServer()
    {
        // Read the IP address from the input field
        serverIP = ipInputField.text;

        if (string.IsNullOrEmpty(serverIP))
        {
            uiManager.UpdateStatus("Please enter a valid IP address.");
            return;
        }

        StartCoroutine(ClientCoroutine()); // Start the client coroutine to connect to the server
    }

    IEnumerator ClientCoroutine()
    {
        client = new TcpClient();
        yield return client.ConnectAsync(serverIP, port); // Connect to the server using the IP address from the input field

        if (client.Connected)
        {
            NetworkStream stream = client.GetStream(); // Get the network stream to receive data

            byte[] fileSizeBytes = new byte[4]; // Array to hold file size (4 bytes)
            stream.Read(fileSizeBytes, 0, 4); // Read file size
            int fileSize = System.BitConverter.ToInt32(fileSizeBytes, 0); // Convert to int

            byte[] fileData = new byte[fileSize]; // Create an array to store the file
            int totalRead = 0;

            // Receive the file in chunks and update progress
            while (totalRead < fileSize)
            {
                int bytesRead = stream.Read(fileData, totalRead, fileSize - totalRead); // Read data
                totalRead += bytesRead;

                float progress = (float)totalRead / fileSize; // Calculate progress
                uiManager.UpdateProgress(progress); // Update progress in UI
            }

            // Save the received file
            File.WriteAllBytes(savePath, fileData);
            uiManager.UpdateStatus("File received and saved!"); // Show success in UI
            client.Close(); // Close the connection
        }
        else
        {
            uiManager.UpdateStatus("Failed to connect to server.");
        }
    }
}
