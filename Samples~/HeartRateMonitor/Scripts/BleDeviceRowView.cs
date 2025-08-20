using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BleDeviceRowView : MonoBehaviour
{
    private string _deviceUuid = string.Empty;
    private string _deviceName = string.Empty;

    [SerializeField] private Text deviceUuidText;
    [SerializeField] private Text deviceNameText;

    [SerializeField] private Image deviceButtonImage;
    [SerializeField] private Text deviceButtonText;

    [SerializeField] private Color onConnectedColor;

    [SerializeField] private Button buttonComponent;
    private Color _previousColor;

    [Header("Events")]
    public UnityEvent<string> onDeviceConnectRequested;

    private bool _isConnected = false;

    public void Show(string uuid, string deviceName)
    {
        deviceButtonText.text = "Connect";

        _deviceUuid = uuid;
        _deviceName = deviceName;

        deviceUuidText.text = uuid;
        deviceNameText.text = deviceName;

        buttonComponent.onClick.AddListener(ToggleConnect);

        gameObject.SetActive(true);
    }

    public void ToggleConnect()
    {
        if (!_isConnected)
        {
            onDeviceConnectRequested?.Invoke(_deviceUuid);
            _isConnected = true;
            return;
        }

        onDeviceConnectRequested?.Invoke(null);
        buttonComponent.interactable = false;
    }

    public void OnDeviceConnected(string deviceUuid)
    {
        _previousColor = deviceButtonImage.color;
        deviceButtonImage.color = onConnectedColor;

        _isConnected = true;
        buttonComponent.interactable = true;
        deviceButtonText.text = "Disconnect";
    }

    public void OnDeviceDisconnected(string deviceUuid)
    {
        deviceButtonImage.color = _previousColor;
        buttonComponent.interactable = true;
        _isConnected = false;
        deviceButtonText.text = "Connect";
    }
}