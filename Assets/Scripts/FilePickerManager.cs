using UnityEngine;
using UnityEngine.UI;

public class FilePickerManager : MonoBehaviour
{
    public FileSender fileSender; // Reference to FileSender script
    public Text selectedFileText; // UI Text to display selected file name

    // Method to open the file picker
    public void PickFile()
    {
        // Open the file picker dialog
        NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                // If file is selected, set the file path in FileSender
                fileSender.filePath = path;

                // Update the UI to show the selected file name
                selectedFileText.text = System.IO.Path.GetFileName(path);
            }
            else
            {
                selectedFileText.text = "No file selected"; // Handle case if no file is selected
            }
        });
    }
}