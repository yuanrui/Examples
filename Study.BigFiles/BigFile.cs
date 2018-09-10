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
        private static Object _syncObj = new Object();
        private static Dictionary<String, Header> _headerDict = new Dictionary<String, Header>();

        public class Header 
        {
            #region Member

            public static readonly Byte[] MagicCode = new Byte[] { 0x59, 0x52, 0x46, 0x53 };

            protected static readonly DateTime StartTime = new DateTime(1, 1, 1);
            
            public const Int32 OFFSET_SIZE = 65536;//64kb

            public const Int32 Int32_SIZE = 4;

            public const Int32 Int64_SIZE = 8;

            public const Int32 CHECKSUM_SIZE = 3;

            public const Int32 BLOCK_HEADER_SIZE = 32;
            
            public Int32 VersionToken;

            public Int64 CurrentOffset;

            public Int64 PrevOffset;

            public Int64 FileCount;

            protected Int64 ActiveTimeValue;

            public DateTime ActiveTime
            {
                get
                {
                    return ToDateTime(ActiveTimeValue);
                }
                set
                {
                    ActiveTimeValue = ToInt64(value);
                }
            }

            public Int64 LastOffset;
            
            protected Int64 LastFileTimeValue;

            public DateTime LastFileTime
            {
                get
                {
                    return ToDateTime(LastFileTimeValue);
                }
                set
                {
                    LastFileTimeValue = ToInt64(value);
                }
            }

            public Int64 CycleTotalFileCount;

            public Int64 OverwriteCount;

            public Int64 FreeStorage;

            public Header()
            {
                ActiveTime = DateTime.MinValue;
                LastFileTime = DateTime.MinValue;
            }

            #endregion

            #region Read

            internal void Read(Stream stream)
            {
                Int64 index = MagicCode.LongLength;
                
                Read(stream, ref VersionToken, ref index);
                Read(stream, ref CurrentOffset, ref index);
                Read(stream, ref PrevOffset, ref index);
                Read(stream, ref FileCount, ref index);
                Read(stream, ref ActiveTimeValue, ref index);
                Read(stream, ref LastOffset, ref index);
                Read(stream, ref LastFileTimeValue, ref index);
                Read(stream, ref CycleTotalFileCount, ref index);
                Read(stream, ref OverwriteCount, ref index);
            }

            private void Read(Stream stream, ref Int32 value, ref Int64 index)
            {
                Byte[] buffer = new Byte[Header.Int32_SIZE];
                stream.Seek(index, SeekOrigin.Begin);
                stream.Read(buffer, 0, buffer.Length);
                index += buffer.Length;
                value = BitConverter.ToInt32(buffer, 0);
            }

            private void Read(Stream stream, ref Int64 value, ref Int64 index)
            {
                Byte[] buffer = new Byte[Header.Int64_SIZE];
                stream.Seek(index, SeekOrigin.Begin);
                stream.Read(buffer, 0, buffer.Length);
                index += buffer.Length;
                value = BitConverter.ToInt64(buffer, 0);
            }

            #endregion

            #region Write

            internal void Write(Stream stream)
            {
                Int64 index = 0L;

                Write(stream, MagicCode, ref index);
                Write(stream, VersionToken, ref index);

                Write(stream, CurrentOffset, ref index);
                Write(stream, PrevOffset, ref index);
                Write(stream, FileCount, ref index);
                Write(stream, ActiveTimeValue, ref index);
                Write(stream, LastOffset, ref index);
                Write(stream, LastFileTimeValue, ref index);
                Write(stream, CycleTotalFileCount, ref index);
                Write(stream, OverwriteCount, ref index);

                stream.Flush();
            }

            private void Write(Stream stream, Int32 value, ref Int64 index)
            {
                Write(stream, BitConverter.GetBytes(value), ref index);
            }

            private void Write(Stream stream, Int64 value, ref Int64 index)
            {
                Write(stream, BitConverter.GetBytes(value), ref index);
            }

            private void Write(Stream stream, Byte[] buffer, ref Int64 index)
            {
                stream.Seek(index, SeekOrigin.Begin);
                stream.Write(buffer, 0, buffer.Length);
                index += buffer.Length;
            }
            
            #endregion

            #region Datetime Help

            public static DateTime ToDateTime(Int64 value)
            {
                return StartTime.AddMilliseconds(value);
            }

            public static Int64 ToInt64(DateTime time)
            {
                Int64 value = (time.Ticks - StartTime.Ticks) / 10000;

                return value;
            }

            #endregion
        }

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

                if (! _headerDict.ContainsKey(_filePath))
                {
                    _headerDict.Add(_filePath, new Header());
                }

                if (_stream.Length >= _length)
                {
                    return;
                }

                _stream.SetLength(_length);

                Header header = GetHeader();
                if (header.CurrentOffset == 0L)
                {
                    header.CurrentOffset = Header.OFFSET_SIZE;
                }
                
                header.VersionToken = Int32.Parse(DateTime.Now.ToString("yyyyMMdd"));
                header.Write(_stream);
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

            if (buffer.LongLength >= (_length - Header.OFFSET_SIZE))
            {
                throw new ArgumentOutOfRangeException("buffer", "文件太大，超出容量。"); 
            }

            Int64 result = 0;
            Byte[] lengthArray = BitConverter.GetBytes(buffer.Length);
            Byte[] timeArray = BitConverter.GetBytes(Header.ToInt64(DateTime.Now));
            Byte lengthChecksum = Checksum(lengthArray);
            Byte dataChecksum = Checksum(buffer);
            Byte timeChecksum = Checksum(timeArray);
            Int64 dataLength = buffer.Length + lengthArray.Length + timeArray.Length + Header.CHECKSUM_SIZE;
            Int64 endOffset = SetOffset(dataLength, DateTime.Now);
            
            result = endOffset - dataLength;

            _stream.Seek(result, SeekOrigin.Begin);
            _stream.Write(lengthArray, 0, lengthArray.Length);

            _stream.Seek(result + lengthArray.Length, SeekOrigin.Begin);
            _stream.Write(timeArray, 0, timeArray.Length);

            _stream.Seek(result + lengthArray.Length + timeArray.Length, SeekOrigin.Begin);
            _stream.WriteByte(lengthChecksum);
            _stream.WriteByte(dataChecksum);
            _stream.WriteByte(timeChecksum);

            _stream.Seek(result + lengthArray.Length + timeArray.Length + Header.CHECKSUM_SIZE, SeekOrigin.Begin);
            _stream.Write(buffer, 0, buffer.Length);

            _stream.Flush();

            return result;
        }

        public Byte[] Read(Int64 offset, out DateTime uploadDate)
        {
            uploadDate = DateTime.Now;
            if (offset >= _length)
            {
                return new Byte[0];
            }
            
            Byte[] lengthArray = new Byte[Header.Int32_SIZE];
            Byte[] timeArray = new Byte[Header.Int64_SIZE];
            Byte[] lengthChecksum = new Byte[1];
            Byte[] dataChecksum = new Byte[1];
            Byte[] timeChecksum = new Byte[1];

            _stream.Seek(offset, SeekOrigin.Begin);
            _stream.Read(lengthArray, 0, lengthArray.Length);
            Int32 dataLength = BitConverter.ToInt32(lengthArray, 0);

            _stream.Seek(offset + lengthArray.Length, SeekOrigin.Begin);
            _stream.Read(timeArray, 0, timeArray.Length);
            
            _stream.Seek(offset + lengthArray.Length + timeArray.Length, SeekOrigin.Begin);
            _stream.Read(lengthChecksum, 0, 1);
            _stream.Read(dataChecksum, 0, 1);
            _stream.Read(timeChecksum, 0, 1);
            
            if (lengthChecksum[0] != Checksum(lengthArray))
            {
                return new Byte[0];
            }

            if (timeArray[0] == Checksum(timeArray))
            {
                Int64 timeValue = BitConverter.ToInt64(timeArray, 0);
                uploadDate = Header.ToDateTime(timeValue);
            }

            Byte[] result = new Byte[dataLength];
            _stream.Seek(offset + lengthArray.Length + timeArray.Length + Header.CHECKSUM_SIZE, SeekOrigin.Begin);
            _stream.Read(result, 0, result.Length);

            if (dataChecksum[0] != Checksum(result))
            {
                return new Byte[0];
            }

            return result;
        }

        public Header GetHeader()
        {
            Header header = _headerDict[_filePath];
            header.Read(_stream);
            header.FreeStorage = _stream.Length - header.CurrentOffset;

            return header;
        }

        private Int64 SetOffset(Int64 offset, DateTime time)
        {
            lock (_syncObj)
            {
                Header header = GetHeader();

                Int64 @newOffset = header.CurrentOffset + offset;

                if (@newOffset > _stream.Length)
                {
                    @newOffset = Header.OFFSET_SIZE + offset;
                    header.LastOffset = header.PrevOffset;
                    header.LastFileTime = header.ActiveTime;
                    header.OverwriteCount = header.OverwriteCount + 1;
                    header.CycleTotalFileCount = header.FileCount;
                    header.FileCount = 0;
                }
                
                header.FileCount = header.FileCount + 1;
                header.PrevOffset = header.CurrentOffset;
                header.CurrentOffset = @newOffset;
                header.ActiveTime = time;
                header.FreeStorage = _stream.Length - header.CurrentOffset;

                header.Write(_stream);

                return @newOffset;
            }
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
