using System;
using Android.BLE;
using Android.BLE.Commands;
using UnityEngine;
using UnityEngine.Events;

namespace Xprees.BLE.HeartMonitor
{
    public class HeartRateService : MonoBehaviour
    {
        // Use full 128-bit UUIDs for better compatibility + custom GATT
        public const string HearRateServiceUUID = "0000180D-0000-1000-8000-00805F9B34FB"; // Heart Rate Service
        public const string HeartRateCharacteristicUUID = "00002A37-0000-1000-8000-00805F9B34FB"; // Heart Rate Measurement

        // Short UUIDs for specific characteristics
        public const string HeartRateServiceShortUUID = "180D"; // Short Heart Rate Service UUID
        public const string HeartRateCharacteristicShortUUID = "2A37"; // Short Heart Rate Measurement Characteristic UUID

        [Tooltip("Reference to Ble Adapter for subscribing to Received Messages.")]
        [SerializeField] private BleAdapter bleAdapter;

        public string ConnectedDevice { get; private set; }
        public bool IsConnected => string.IsNullOrEmpty(ConnectedDevice) == false;

        [Header("Events")]
        public UnityEvent<HeartRateData> onHeartRateUpdated;

        public UnityEvent<string> onDeviceConnected;
        public UnityEvent<string> onDeviceDisconnected;

        private ConnectToDevice _connectCommand;
        private SubscribeToCharacteristic _subscribeToCharacteristicCommand;

        private void Awake()
        {
            // Try to get BleAdapter from the same game object
            if (!bleAdapter) bleAdapter = GetComponent<BleAdapter>();
            if (!bleAdapter) Debug.LogWarning("Couldn't hook to BleAdapter OnMessageReceived. Adapter not found!");
        }

        private void OnEnable()
        {
            if (bleAdapter) bleAdapter.OnMessageReceived += OnDataReceived;
        }

        private void OnDisable()
        {
            if (bleAdapter) bleAdapter.OnMessageReceived -= OnDataReceived;

            DisconnectFromDevice();
        }

        /// Processes the received data from the BLE device.
        /// This method checks if the data is from the expected Heart Rate Service and Characteristic,
        /// and then extracts the heart rate data from the byte message.
        /// If the data is not from the expected service or characteristic, it logs an error.
        public void OnDataReceived(BleObject data)
        {
            if (data == null || data.Service == null || data.Characteristic == null) return;

            var dataService = data.Service;
            if (!IsHeartRateService(dataService))
            {
                Debug.LogWarning($"filtering out data from unexpected service: {dataService}");
                return;
            }

            var characteristic = data.Characteristic;
            if (!IsHeartRateCharacteristic(characteristic))
            {
                Debug.LogError($"Received data for unexpected characteristic: {characteristic}");
                return;
            }

            if (data.HasError)
            {
                Debug.LogError($"Error receiving data: {data.ErrorMessage}");
                return;
            }

            OnHeartDataReceived(data.GetByteMessage());
        }

        /// Connects to the Heart Rate Service on a BLE device.
        /// <param name="deviceUuid">UUID of the Heart rate monitor</param>
        public void ConnectToDevice(string deviceUuid)
        {
            if (string.IsNullOrEmpty(deviceUuid))
            {
                Debug.LogError("Device UUID is null or empty. Cannot connect to Heart Rate Service.");
                return;
            }

            _connectCommand = new ConnectToDevice(deviceUuid, OnConnected, OnDisconnected);
            BleManager.Instance.QueueCommand(_connectCommand);
        }

        /// Disconnects and Unsubscribes from the Heart Rate Monitor BLE device.
        public void DisconnectFromDevice()
        {
            if (!IsConnected) return;

            UnsubscribeFromHeartService();
            _connectCommand.Disconnect();
        }

        public void SubscribeToHeartService(string deviceUuid)
        {
            _subscribeToCharacteristicCommand = new SubscribeToCharacteristic(deviceUuid, HearRateServiceUUID,
                HeartRateCharacteristicUUID, OnHeartDataReceived, true);

            _subscribeToCharacteristicCommand.Start();
            Debug.Log($"Subscribed to Heart Rate Service for device: {deviceUuid}");
        }

        /// Only Unsubscribes from the Heart Rate Service, does not disconnect from the device.
        public void UnsubscribeFromHeartService()
        {
            _subscribeToCharacteristicCommand?.Unsubscribe();
            _subscribeToCharacteristicCommand = null;
            Debug.Log("Unsubscribed from Heart Rate Service");
        }

        private void OnConnected(string deviceUuid)
        {
            ConnectedDevice = deviceUuid;
            onDeviceConnected?.Invoke(deviceUuid);
            SubscribeToHeartService(deviceUuid);
            Debug.Log($"Connected to Heart Rate Service on device: {deviceUuid}");
        }

        private void OnDisconnected(string deviceUuid)
        {
            ConnectedDevice = null;
            onDeviceDisconnected?.Invoke(deviceUuid);
            UnsubscribeFromHeartService();
            Debug.Log($"Disconnected to device: {deviceUuid}");
        }

        private void OnHeartDataReceived(byte[] rawData)
        {
            try
            {
                var heartRateData = HeartRateData.ExtractFromRawData(rawData);
                if (!heartRateData.HasValue)
                {
                    Debug.LogWarning($"Failed to extract heart rate data from raw data {rawData}");
                    return;
                }

                onHeartRateUpdated?.Invoke(heartRateData.Value);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing heart rate data: {e.Message}");
            }
        }

        public static bool IsHeartRateCharacteristic(string characteristic) =>
            characteristic.Equals(HeartRateCharacteristicShortUUID, StringComparison.InvariantCultureIgnoreCase)
            || characteristic.Equals(HeartRateCharacteristicUUID, StringComparison.InvariantCultureIgnoreCase);

        public static bool IsHeartRateService(string dataService) =>
            dataService.Equals(HearRateServiceUUID, StringComparison.InvariantCultureIgnoreCase)
            || dataService.Equals(HeartRateServiceShortUUID, StringComparison.InvariantCultureIgnoreCase);
    }
}