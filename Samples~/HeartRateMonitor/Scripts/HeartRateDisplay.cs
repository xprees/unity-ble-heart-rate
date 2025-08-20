using UnityEngine;
using UnityEngine.UI;
using Xprees.BLE.HeartMonitor;

public class HeartRateDisplay : MonoBehaviour
{
    [SerializeField] private Text heartRateText;

    public void OnHeartDataReceived(HeartRateData data)
    {
        if (heartRateText == null)
        {
            Debug.LogError("HeartRateText is not assigned in the HeartRateDisplay component.");
            return;
        }

        // Display the heart rate in BPM
        heartRateText.text = $"Heart Rate: {data.BPM} BPM";

        // Optionally, display RR intervals if needed
        if (data.RRIntervals is not { Length: > 0 }) return;

        var rrIntervals = string.Join(", ", data.RRIntervals);
        heartRateText.text += $"\nRR Intervals: {rrIntervals} ms";
    }
}