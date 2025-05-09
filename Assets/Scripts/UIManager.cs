using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text statusText; // Status text to show messages
    public Slider progressSlider; // Slider to display progress
    public Text percentText;
    
    public void UpdateStatus(string message)
    {
        statusText.text = message; // Update the status text with the given message
    }

    public void UpdateProgress(float progress)
    {
        progressSlider.value = progress; // Update the progress slider with the given value
        percentText.text = progress.ToString("P");
    }
}