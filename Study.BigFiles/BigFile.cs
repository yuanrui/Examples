using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Study.BigFiles
{
    public class BigFile : IDisposable
    {
        private String _filePath;
        private Int64 _length;
        private Stream _stream;
        protected Int32 HEAD_OFFSET_SIZE = sizeof(Int64);
        protected Int32 BUFFER_OFFSET_SIZE = sizeof(Int32);
        protected const Int32 CHECKSUM_SIZE = 2;
        public static Object _syncObj = new Object();
        [ThreadStatic]
        protected static Int64 _currentOffset;

        public BigFile(String filePath, Int64 length)
        {
            _filePath = filePath;
            _length = length;

            Init();
        }

        protected void Init()
        {
            lock (_syncObj)
            {
                _stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                if (_stream.Length == _length)
                {
                    return;
                }

                _stream.SetLength(_length);

                SetOffset(HEAD_OFFSET_SIZE);
            }
        }

        public void Dispose()
        {
            if (_stream == null)
            {
                return;
            }

            _stream.Dispose();
        }

        public Int64 Write(Byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (buffer.LongLength >= (_length - HEAD_OFFSET_SIZE))
            {
                throw new ArgumentOutOfRangeException("buffer", "文件太大，超出容量。"); 
            }

            Int64 result = 0;
            Byte[] lengthArray = BitConverter.GetBytes(buffer.Length);
            Byte lengthChecksum = Checksum(lengthArray);
            Byte dataChecksum = Checksum(buffer);
            Int64 dataLength = buffer.Length + lengthArray.Length + CHECKSUM_SIZE;
            Int64 endOffset = SetOffset(dataLength);
            
            result = endOffset - dataLength;

            _stream.Seek(result, SeekOrigin.Begin);
            _stream.Write(lengthArray, 0, lengthArray.Length);

            _stream.Seek(result + lengthArray.Length, SeekOrigin.Begin);
            _stream.WriteByte(lengthChecksum);
            _stream.WriteByte(dataChecksum);

            _stream.Seek(result + lengthArray.Length + CHECKSUM_SIZE, SeekOrigin.Begin);
            _stream.Write(buffer, 0, buffer.Length);

            _stream.Flush();

            return result;
        }

        public Byte[] Read(Int64 offset)
        {
            Byte[] lengthArray = new Byte[BUFFER_OFFSET_SIZE];
            Byte[] lengthChecksum = new Byte[1];
            Byte[] dataChecksum = new Byte[1];

            _stream.Seek(offset, SeekOrigin.Begin);
            _stream.Read(lengthArray, 0, lengthArray.Length);
            Int32 dataLength = BitConverter.ToInt32(lengthArray, 0);
            _stream.Seek(offset + lengthArray.Length, SeekOrigin.Begin);
            _stream.Read(lengthChecksum, 0, 1);
            _stream.Read(dataChecksum, 0, 1);

            if (lengthChecksum[0] != Checksum(lengthArray))
            {
                return new Byte[0];
            }

            Byte[] result = new Byte[dataLength];
            _stream.Seek(offset + lengthArray.Length + CHECKSUM_SIZE, SeekOrigin.Begin);
            _stream.Read(result, 0, result.Length);

            if (dataChecksum[0] != Checksum(result))
            {
                return new Byte[0];
            }

            return result;
        }

        private Int64 SetOffset(Int64 offset)
        {
            lock (_syncObj)
            {
                _currentOffset = GetOffset();
                Int64 @newOffset = _currentOffset + offset;

                if (@newOffset > _stream.Length)
                {
                    @newOffset = HEAD_OFFSET_SIZE + offset;
                }

                _currentOffset = @newOffset;

                Byte[] offsetArray = BitConverter.GetBytes(@newOffset);
                _stream.Seek(0L, SeekOrigin.Begin);
                _stream.Write(offsetArray, 0, offsetArray.Length);
                _stream.Flush();

                return @newOffset;
            }
        }

        private Int64 GetOffset()
        {
            Byte[] lengthArray = new Byte[HEAD_OFFSET_SIZE];
            _stream.Seek(0L, SeekOrigin.Begin);
            _stream.Read(lengthArray, 0, lengthArray.Length);

            return BitConverter.ToInt64(lengthArray, 0);
        }

        private static Byte Checksum(Byte[] data)
        {
            Byte sum = 0;
            unchecked
            {
                foreach (Byte b in data)
                {
                    sum += b;
                }
            }
            return sum;
        }
    }
}
