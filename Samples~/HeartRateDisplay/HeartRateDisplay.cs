using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xprees.BLE.HeartMonitor;

public class HeartRateDisplay : MonoBehaviour
{
    public TextMeshProUGUI heartRateText;
    public Image heartIcon;

    [Header("Settings")]
    public float iconScaleFactor = 1.2f;

    public Color iconNormalColor = Color.red;

    [Space]
    public Color textNormalColor = Color.white;

    [Space]
    public Color disconnectedColor = Color.gray;

    private bool _isConnected = false;

    private void Start()
    {
        OnDeviceDisconnected();
    }

    public void OnHeartDataReceived(HeartRateData data)
    {
        if (heartRateText == null)
        {
            Debug.LogWarning("HeartRateText is not assigned in the HeartRateDisplay component.");
            return;
        }

        heartRateText.text = $"{data.BPM} bpm";
    }

    public void OnDeviceConnected()
    {
        _isConnected = true;
        if (heartIcon) heartIcon.color = iconNormalColor;
        if (heartRateText) heartRateText.color = textNormalColor;
    }

    public void OnDeviceDisconnected()
    {
        _isConnected = false;
        if (heartIcon) heartIcon.color = disconnectedColor;
        if (heartRateText)
        {
            heartRateText.color = disconnectedColor;
            heartRateText.text = "-- bpm";
        }
    }


    private void LateUpdate()
    {
        if (!heartIcon) return;

        float scale = 1;
        if (_isConnected)
        {
            scale = 1 + Mathf.PingPong(Time.time * 2, 0.4f) * (iconScaleFactor - 1);
        }

        heartIcon.transform.localScale = new Vector3(scale, scale, scale);
    }
}