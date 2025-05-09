using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FilePickerManager : MonoBehaviour
{
    public FileSender fileSender; // Reference to FileSender script
    public Text selectedFileText; // UI Text to display selected file names

    // Method to open the file picker for one or multiple files
    public void PickFile()
    {
        // Check if the device supports picking multiple files
        if (NativeFilePicker.CanPickMultipleFiles())
        {
            // Open the file picker dialog for multiple files
            NativeFilePicker.PickMultipleFiles((string[] paths) =>
            {
                if (paths != null && paths.Length > 0)
                {
                    // Convert array to List for FileSender if needed
                    fileSender.filePaths = paths.ToList();

                    // Create a string to display all selected file names
                    string fileNames = "Selected Files:\n";
                    foreach (var path in paths)
                    {
                        fileNames += System.IO.Path.GetFileName(path) + "\n";
                    }

                    selectedFileText.text = fileNames; // Update UI to show file names
                }
                else
                {
                    selectedFileText.text = "No files selected"; // Handle case if no files are selected
                }
            });
        }
        else
        {
            // If multiple file selection is not supported, use single file selection
            selectedFileText.text = "Multiple file selection is not supported on this device. Use single file instead.";

            NativeFilePicker.PickFile((path) =>
            {
                if (path != null)
                {
                    // If a single file is selected, set the file path in FileSender
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
}
