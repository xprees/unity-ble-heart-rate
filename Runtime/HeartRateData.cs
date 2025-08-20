using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xprees.BLE.HeartMonitor
{
    /// Simple data structure to hold heart rate data from the BLE device.
    [Serializable]
    public struct HeartRateData
    {
        /// Beats per minute (BPM) and RR intervals in milliseconds
        public int BPM { get; set; }

        /// RR intervals in milliseconds
        public int[] RRIntervals { get; set; }

        /// Factory method to extract heart rate data from raw byte array.
        /// <param name="rawData">Converted received data to bytes</param>
        public static HeartRateData? ExtractFromRawData(byte[] rawData)
        {
            if (rawData == null || rawData.Length <= 2)
            {
                var rawDataString = rawData != null ? BitConverter.ToString(rawData) : "null";
                Debug.LogError($"Invalid heart rate data received: {rawDataString}");
                return null;
            }

            var heartRateData = new HeartRateData
            {
                BPM = ExtractBpm(rawData),
                RRIntervals = ExtractRrIntervals(rawData)?.ToArray() ?? Array.Empty<int>()
            };

            return heartRateData;
        }

        private static int ExtractBpm(byte[] data)
        {
            var flags = data[0];
            if ((flags & 1) == 0)
            {
                return data[1];
            }

            return data[1] | (data[2] << 8);
        }

        private static List<int> ExtractRrIntervals(byte[] data)
        {
            var rrIntervals = new List<int>();
            var flags = data[0];

            // Check if RR intervals are present (bit 4 of flags)
            if ((flags & 0x10) == 0) return rrIntervals;

            var offset = (flags & 1) == 0 ? 2 : 3; // Start after HR value

            while (offset < data.Length - 1)
            {
                var rr = data[offset] | (data[offset + 1] << 8);
                rrIntervals.Add(rr); // Value in 1/1024 seconds
                offset += 2;
            }

            return rrIntervals;
        }
    }
}