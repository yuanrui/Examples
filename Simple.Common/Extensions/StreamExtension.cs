using System;
using System.IO;

namespace Simple.Common.Extensions
{
    public static class StreamExtension
    {
        public static Byte[] ReadBytes(this Stream stream, Int32 length)
        {
            Byte[] result = new Byte[length];
            Int32 offset = 0;
            
            while (offset < length)
            {
                var increment = stream.Read(result, offset, length - offset);
                if (increment == 0)
                {
                    throw new EndOfStreamException();
                }

                offset += increment;
            }

            return result;
        }

        public static void ReadBytesTo(this Stream stream, Byte[] buffer)
        {
            Int32 length = buffer.Length;
            Int32 offset = 0;

            while (offset < length)
            {
                var increment = stream.Read(buffer, offset, length - offset);
                if (increment == 0)
                {
                    throw new EndOfStreamException();
                }

                offset += increment;
            }
        }
    }
}
