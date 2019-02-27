using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Simple.Common.Extensions;

namespace Simple.Common.Cryptography
{
    class CryptoUtils
    {
        // Fixed Byte pattern for the OID header
        static readonly Byte[] OIDHeader = { 0x30, 0xD, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x1, 0x1, 0x5, 0x0 };

        public static String PublicKeyFingerprInt32(RSAParameters parameters)
        {
            // Public key fingerprInt32 is defined as the SHA1 of the modulus + exponent Bytes
            return SHA1Hash(parameters.Modulus.Append(parameters.Exponent).ToArray());
        }

        public static String EncodePEMPublicKey(RSAParameters parameters)
        {
            var data = Convert.ToBase64String(EncodePublicKey(parameters));
            var output = new StringBuilder();
            output.AppendLine("-----BEGIN PUBLIC KEY-----");
            for (var i = 0; i < data.Length; i += 64)
                output.AppendLine(data.Substring(i, Math.Min(64, data.Length - i)));
            output.Append("-----END PUBLIC KEY-----");

            return output.ToString();
        }

        public static RSAParameters DecodePEMPublicKey(String key)
        {
            try
            {
                // Reconstruct original key data
                var lines = key.Split('\n');
                var data = Convert.FromBase64String(String.Join(String.Empty, lines.Skip(1).Take(lines.Length - 2)));

                // Pull the modulus and exponent Bytes out of the ASN.1 tree
                // Expect this to blow up if the key is not correctly formatted
                using (var s = new MemoryStream(data))
                {
                    // SEQUENCE
                    s.ReadByte();
                    ReadTLVLength(s);

                    // SEQUENCE -> fixed header junk
                    s.ReadByte();
                    var headerLength = ReadTLVLength(s);
                    s.Position += headerLength;

                    // SEQUENCE -> BIT_STRING
                    s.ReadByte();
                    ReadTLVLength(s);
                    s.ReadByte();

                    // SEQUENCE -> BIT_STRING -> SEQUENCE
                    s.ReadByte();
                    ReadTLVLength(s);

                    // SEQUENCE -> BIT_STRING -> SEQUENCE -> INTEGER (modulus)
                    s.ReadByte();
                    var modulusLength = ReadTLVLength(s);
                    s.ReadByte();
                    var modulus = s.ReadBytes(modulusLength - 1);
                    
                    // SEQUENCE -> BIT_STRING -> SEQUENCE -> INTEGER (exponent)
                    s.ReadByte();
                    var exponentLength = ReadTLVLength(s);
                    s.ReadByte();
                    var exponent = s.ReadBytes(exponentLength - 1);

                    return new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Invalid PEM public key", e);
            }
        }

        static Byte[] EncodePublicKey(RSAParameters parameters)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                var modExpLength = TripletFullLength(parameters.Modulus.Length + 1) + TripletFullLength(parameters.Exponent.Length + 1);
                var bitStringLength = TripletFullLength(modExpLength + 1);
                var sequenceLength = TripletFullLength(bitStringLength + OIDHeader.Length);

                // SEQUENCE
                writer.Write((Byte)0x30);
                WriteTLVLength(writer, sequenceLength);

                // SEQUENCE -> fixed header junk
                writer.Write(OIDHeader);

                // SEQUENCE -> BIT_STRING
                writer.Write((Byte)0x03);
                WriteTLVLength(writer, bitStringLength);
                writer.Write((Byte)0x00);

                // SEQUENCE -> BIT_STRING -> SEQUENCE
                writer.Write((Byte)0x30);
                WriteTLVLength(writer, modExpLength);

                // SEQUENCE -> BIT_STRING -> SEQUENCE -> INTEGER
                // Modulus is padded with a zero to avoid issues with the sign bit
                writer.Write((Byte)0x02);
                WriteTLVLength(writer, parameters.Modulus.Length + 1);
                writer.Write((Byte)0);
                writer.Write(parameters.Modulus);

                // SEQUENCE -> BIT_STRING -> SEQUENCE -> INTEGER
                // Exponent is padded with a zero to avoid issues with the sign bit
                writer.Write((Byte)0x02);
                WriteTLVLength(writer, parameters.Exponent.Length + 1);
                writer.Write((Byte)0);
                writer.Write(parameters.Exponent);

                return stream.ToArray();
            }
        }

        static void WriteTLVLength(BinaryWriter writer, Int32 length)
        {
            if (length < 0x80)
            {
                // Length < 128 is stored in a single Byte
                writer.Write((Byte)length);
            }
            else
            {
                // If 128 <= length < 256**128 first Byte encodes number of Bytes required to hold the length
                // High-bit is set as a flag to use this long-form encoding
                var lengthBytes = BitConverter.GetBytes(length).Reverse().SkipWhile(b => b == 0).ToArray();
                writer.Write((Byte)(0x80 | lengthBytes.Length));
                writer.Write(lengthBytes);
            }
        }

        static Int32 ReadTLVLength(Stream s)
        {
            var length = s.ReadByte();
            if (length < 0x80)
                return length;

            var data = new Byte[4];
            s.ReadBytes(data, 0, Math.Min(length & 0x7F, 4));
            return BitConverter.ToInt32(data.ToArray(), 0);
        }

        static Int32 TripletFullLength(Int32 dataLength)
        {
            if (dataLength < 0x80)
                return 2 + dataLength;

            return 2 + dataLength + BitConverter.GetBytes(dataLength).Reverse().SkipWhile(b => b == 0).Count();
        }

        public static String DecryptString(RSAParameters parameters, String data)
        {
            try
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(parameters);
                    return Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(data), false));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Failed to decrypt String with exception: " + e, "debug");
                Console.WriteLine("String decryption failed: {0}", e);
                return null;
            }
        }

        public static String Sign(RSAParameters parameters, String data)
        {
            return Sign(parameters, Encoding.UTF8.GetBytes(data));
        }

        public static String Sign(RSAParameters parameters, Byte[] data)
        {
            try
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(parameters);
                    using (var csp = SHA1.Create())
                        return Convert.ToBase64String(rsa.SignHash(csp.ComputeHash(data), CryptoConfig.MapNameToOID("SHA1")));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Failed to sign String with exception: " + e, "debug");
                Console.WriteLine("String signing failed: {0}", e);
                return null;
            }
        }

        public static bool VerifySignature(RSAParameters parameters, String data, String signature)
        {
            return VerifySignature(parameters, Encoding.UTF8.GetBytes(data), signature);
        }

        public static bool VerifySignature(RSAParameters parameters, Byte[] data, String signature)
        {
            try
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(parameters);
                    using (var csp = SHA1.Create())
                        return rsa.VerifyHash(csp.ComputeHash(data), CryptoConfig.MapNameToOID("SHA1"), Convert.FromBase64String(signature));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Failed to verify signature with exception: " + e, "debug");
                Console.WriteLine("Signature validation failed: {0}", e);
                return false;
            }
        }

        public static String SHA1Hash(Stream data)
        {
            using (var csp = SHA1.Create())
                return new String(csp.ComputeHash(data).SelectMany(a => a.ToString("x2")).ToArray());
        }

        public static String SHA1Hash(Byte[] data)
        {
            using (var csp = SHA1.Create())
                return new String(csp.ComputeHash(data).SelectMany(a => a.ToString("x2")).ToArray());
        }

        public static String SHA1Hash(String data)
        {
            return SHA1Hash(Encoding.UTF8.GetBytes(data));
        }
    }
}
