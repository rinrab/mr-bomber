// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;

namespace MrBoom.Server
{
    public interface IMetrics
    {
        void PacketReceived(IPEndPoint ip, int len);
        void PacketSent(IPEndPoint ip, int len);

        IReadOnlyDictionary<IPEndPoint, MetricsEntry> GetMetrics();
    }

    public class MetricsEntry
    {
        public long PacketsReceived { get; set; }
        public long BytesReceived { get; set; }

        public double AverageBytesReceived
        {
            get
            {
                if (PacketsReceived == 0)
                {
                    return 0;
                }
                else
                {
                    return BytesReceived / PacketsReceived;
                }
            }
        }

        public int PacketsSent { get; set; }
        public int BytesSent { get; set; }

        public double AverageBytesSent
        {
            get
            {
                if (PacketsSent == 0)
                {
                    return 0;
                }
                else
                {
                    return BytesSent / PacketsSent;
                }
            }
        }
    }

    public class Metrics : IMetrics
    {
        private Dictionary<IPEndPoint, MetricsEntry> metrics;

        public Metrics()
        {
            metrics = new Dictionary<IPEndPoint, MetricsEntry>();
        }

        private MetricsEntry getEntry(IPEndPoint ip)
        {
            if (metrics.TryGetValue(ip, out MetricsEntry? entry))
            {
                return entry;
            }
            else
            {
                MetricsEntry newEntry = new MetricsEntry();
                metrics.Add(ip, newEntry);
                return newEntry;
            }
        }

        public void PacketReceived(IPEndPoint ip, int len)
        {
            var entry = getEntry(ip);

            entry.PacketsReceived++;
            entry.BytesReceived += len;
        }

        public void PacketSent(IPEndPoint ip, int len)
        {
            var entry = getEntry(ip);

            entry.PacketsSent++;
            entry.BytesSent += len;
        }

        public IReadOnlyDictionary<IPEndPoint, MetricsEntry> GetMetrics()
        {
            return metrics;
        }
    }
}
