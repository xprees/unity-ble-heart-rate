using UnityEngine;
using Xprees.BLE.HeartMonitor;

public class BleListUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private BleDeviceRowView deviceButton;

    [SerializeField] private Transform deviceList;

    public void InstantiateDeviceUI(string uuid, string deviceName)
    {
        var button = Instantiate(deviceButton, deviceList);
        button.Show(uuid, deviceName);
    }
}