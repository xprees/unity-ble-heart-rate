using Android.BLE;
using Android.BLE.Commands;
using UnityEngine;
using UnityEngine.Events;

namespace Xprees.BLE.HeartMonitor
{
    /// Scans for BLE devices.
    public class BleDiscoverer : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("How long scan for the new BLE devices.")]
        [SerializeField] private int scanTime = 10;

        [Header("Events")]
        [Tooltip("Event fired when new device discovered. Arguments (UUID, DeviceName)")]
        public UnityEvent<string, string> onDeviceFound;

        [Tooltip("Fires when discovery period ends.")]
        public UnityEvent onDiscoveryFinished;

        private float _scanTimer = 0f;
        private bool _isScanning = false;

        public void ScanForDevices()
        {
            if (_isScanning) return;

            _isScanning = true;
            BleManager.Instance.QueueCommand(new DiscoverDevices(OnDeviceFound, OnFinishedDiscovering,
                scanTime * 1000));
        }

        private void Update() => UpdateScanningTimer();

        private void UpdateScanningTimer()
        {
            if (!_isScanning) return;

            _scanTimer += Time.deltaTime;
            if (_scanTimer > scanTime)
            {
                _scanTimer = 0f;
                _isScanning = false;
            }
        }

        private void OnDeviceFound(string uuid, string deviceName)
        {
            onDeviceFound?.Invoke(uuid, deviceName);
        }

        private void OnFinishedDiscovering() => onDiscoveryFinished?.Invoke();
    }
}