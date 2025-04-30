using System.Collections;
using UnityEngine;
using System.IO;
using System.Net.Sockets;

public class FileReceiver : MonoBehaviour
{
    public string savePath = "received_file"; // Where the file will be saved
    public string serverIP = "127.0.0.1"; // Default server IP
    public int port = 8888; // Port of the server
    public UIManager uiManager; // Reference to UIManager to update progress

    private TcpClient client; // Client to connect to the server

    public void ConnectToServer()
    {
        StartCoroutine(ClientCoroutine()); // Start the client coroutine to connect to the server
    }

    IEnumerator ClientCoroutine()
    {
        client = new TcpClient();
        yield return client.ConnectAsync(serverIP, port); // Connect to the server

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
