using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Simple.Common.Net.NetworkInformation
{
    /// <summary>
    /// Contains methods to validate the semantics of IPv4 addresses
    /// </summary>
    public static class IPSemantics
    {
        #region IP

        /// <summary>
        /// Determines whether the specified IP is a valid IP.
        /// </summary>
        /// <param name="ip">The IP.</param>
        /// <returns>
        /// 	<c>true</c> if the specified IP is a valid IP; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidIP(IPAddress ip)
        {
            return IsValidIP(ip, IPRanges.HttpInvalid);
        }

        /// <summary>
        /// Determines whether the specified IP is a valid IP.
        /// </summary>
        /// <param name="ip">The IP.</param>
        /// <param name="invalidRange">The invalid range.</param>
        /// <returns>
        /// 	<c>true</c> if the specified IP is a valid IP; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidIP(IPAddress ip, IPRanges invalidRange)
        {
            return (GetIPRange(ip) & invalidRange) == IPRanges.None;
        }

        /// <summary>
        /// Gets the IP range to which an IP address belongs.
        /// </summary>
        /// <param name="ip">The IP.</param>
        /// <returns></returns>
        public static IPRanges GetIPRange(IPAddress ip)
        {
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 0, 0, 0, 0 }),
                new IPAddress(new byte[] { 255, 0, 0, 0 })))
                return IPRanges.Current;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 10, 0, 0, 0 }),
                new IPAddress(new byte[] { 255, 0, 0, 0 })))
                return IPRanges.Private10;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 14, 0, 0, 0 }),
                new IPAddress(new byte[] { 255, 0, 0, 0 })))
                return IPRanges.PublicData;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 127, 0, 0, 0 }),
                new IPAddress(new byte[] { 255, 0, 0, 0 })))
                return IPRanges.Loopback;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 128, 0, 0, 0 }),
                new IPAddress(new byte[] { 255, 255, 0, 0 })))
                return IPRanges.Reserved128;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 169, 254, 0, 0 }),
                new IPAddress(new byte[] { 255, 255, 0, 0 })))
                return IPRanges.LinkLocal;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 172, 16, 0, 0 }),
                new IPAddress(new byte[] { 255, 240, 0, 0 })))
                return IPRanges.Private172;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 191, 255, 0, 0 }),
                new IPAddress(new byte[] { 255, 255, 0, 0 })))
                return IPRanges.Reserved191;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 192, 0, 0, 0 }),
                new IPAddress(new byte[] { 255, 255, 255, 0 })))
                return IPRanges.Reserved192;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 192, 0, 2, 0 }),
                new IPAddress(new byte[] { 255, 255, 255, 0 })))
                return IPRanges.Documentation;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 192, 88, 99, 0 }),
                new IPAddress(new byte[] { 255, 255, 255, 0 })))
                return IPRanges.IPv6ToIPv4;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 192, 168, 0, 0 }),
                new IPAddress(new byte[] { 255, 255, 0, 0 })))
                return IPRanges.Private192;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 198, 18, 0, 0 }),
                new IPAddress(new byte[] { 255, 254, 0, 0 })))
                return IPRanges.BenchmarkTest;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 223, 255, 255, 0 }),
                new IPAddress(new byte[] { 255, 255, 255, 0 })))
                return IPRanges.Reserved223;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 224, 0, 0, 0 }),
                new IPAddress(new byte[] { 240, 0, 0, 0 })))
                return IPRanges.Multicast;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 240, 0, 0, 0 }),
                new IPAddress(new byte[] { 240, 0, 0, 0 })))
                return IPRanges.Reserved240;
            if (checkIpRange(
                ip,
                new IPAddress(new byte[] { 255, 255, 255, 255 }),
                new IPAddress(new byte[] { 255, 255, 255, 255 })))
                return IPRanges.Broadcast;

            return IPRanges.None;
        }

        private static bool checkIpRange(IPAddress ip, IPAddress subnet, IPAddress mask)
        {
            if (!(ip.AddressFamily == AddressFamily.InterNetwork))
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.InvariantCulture, "Unsupported IP type: {0}", ip.AddressFamily), "ip");
            }
            for (int i = 0; i < 4; i++)
            {
                if ((ip.GetAddressBytes()[i] & mask.GetAddressBytes()[i] ^ subnet.GetAddressBytes()[i]) != 0)
                    return false;
            }
            return true;
        }

        #endregion
    }

    /// <summary>
    /// Flags that indicate well-known IP address ranges (default only HTTP-valid Ips)
    /// See http://en.wikipedia.org/wiki/IPv4#Addressing
    /// </summary>
    [Flags]
    public enum IPRanges
    {
        /// <summary>
        /// No addresses 
        /// </summary>
        None = 0x00000,
        /// <summary>
        /// 0.0.0.0/8 - Current network (only valid as source address) - RFC 1700
        /// </summary>
        Current = 0x00001,
        /// <summary>
        /// 10.0.0.0/8 - Private network - RFC 1918
        /// </summary>
        Private10 = 0x00002,
        /// <summary>
        /// 14.0.0.0/8 - Public data networks - RFC 1700
        /// </summary>
        PublicData = 0x00004,
        /// <summary>
        /// 127.0.0.0/8 - Loopback - RFC 3330
        /// </summary>
        Loopback = 0x0008,
        /// <summary>
        /// 128.0.0.0/16 - Reserved (IANA) - RFC 3330
        /// </summary>
        Reserved128 = 0x00010,
        /// <summary>
        /// 169.254.0.0/16 - Link-Local - RFC 3927
        /// </summary>
        LinkLocal = 0x00020,
        /// <summary>
        /// 172.16.0.0/12 - Private network - RFC 1918
        /// </summary>
        Private172 = 0x00040,
        /// <summary>
        /// 191.255.0.0/16 - Reserved (IANA) - RFC 3330
        /// </summary>
        Reserved191 = 0x00080,
        /// <summary>
        /// 192.0.0.0/24 - Reserved (IANA) - RFC 3330
        /// </summary>
        Reserved192 = 0x00100,
        /// <summary>
        /// 192.0.2.0/24 - Documentation and example code - RFC 3330
        /// </summary>
        Documentation = 0x00200,
        /// <summary>
        /// 192.88.99.0/24 - IPv6 to IPv4 relay - RFC 3068
        /// </summary>
        IPv6ToIPv4 = 0x00400,
        /// <summary>
        /// 192.168.0.0/16 - Private network - RFC 1918
        /// </summary>
        Private192 = 0x00800,
        /// <summary>
        /// 198.18.0.0/15 - Network benchmark tests - RFC 2544
        /// </summary>
        BenchmarkTest = 0x01000,
        /// <summary>
        /// 223.255.255.0/24 - Reserved (IANA) - RFC 3330
        /// </summary>
        Reserved223 = 0x02000,
        /// <summary>
        /// 224.0.0.0/4 - Multicasts (former Class D network) - RFC 3171
        /// </summary>
        Multicast = 0x04000,
        /// <summary>
        /// 240.0.0.0/4 - Reserved (former Class E network) - RFC 1700
        /// </summary>
        Reserved240 = 0x08000,
        /// <summary>
        /// 255.255.255.255 - Broadcast
        /// </summary>
        Broadcast = 0x10000,

        /// <summary>
        /// Private Network IPs
        /// </summary>
        Private = Private10 | Private172 | Private192 | Loopback,
        /// <summary>
        /// Reserved IPs
        /// </summary>
        Reserved = Reserved128 | Reserved191 | Reserved192 | Reserved223 | Reserved240,
        /// <summary>
        /// Multicast and Broadcast IPs
        /// </summary>
        Cast = Multicast | Broadcast,
        /// <summary>
        /// IP-internal addresses
        /// </summary>
        Internal = Current | PublicData | LinkLocal | Documentation | IPv6ToIPv4 | BenchmarkTest,

        /// <summary>
        /// Addresses that are invalid for normal HTTP operation (default)
        /// </summary>
        HttpInvalid = Reserved | Cast | Internal,

        /// <summary>
        /// All non public addresses
        /// </summary>
        NotPublic = HttpInvalid | Private
    }
}
