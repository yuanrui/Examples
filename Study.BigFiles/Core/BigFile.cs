using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Study.BigFiles
{
    public class BigFile : IDisposable
    {
        private readonly String _filePath;
        private readonly Int64 _length;
        private Stream _stream;
        private static Object _syncObj = new Object();
        private static Dictionary<String, Header> _headerDict = new Dictionary<String, Header>();
        public Boolean IsDisposed { get; private set; }

        public class Header
        {
            #region Member

            public static readonly Byte[] MagicCode = new Byte[] { 0x59, 0x52, 0x46, 0x53 };

            protected static readonly DateTime StartTime = new DateTime(1, 1, 1);

            public const Int32 OFFSET_SIZE = 65536;//64kb

            public const Int32 Int32_SIZE = 4;

            public const Int32 Int64_SIZE = 8;

            public const Int32 BLOCK_HEADER_SIZE = 64;

            public Boolean FeatureMatch;

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

            public Int64 FinalOffset;

            protected Int64 LastFileTimeValue;

            public DateTime FinalFileTime
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

            protected Int64 OverwriteTimeValue;

            public DateTime OverwriteTime
            {
                get
                {
                    return ToDateTime(OverwriteTimeValue);
                }
                set
                {
                    OverwriteTimeValue = ToInt64(value);
                }
            }

            public Int64 FreeStorage;

            public Header()
            {
                ActiveTime = default(DateTime);
                FinalFileTime = default(DateTime);
                OverwriteTime = default(DateTime);
            }

            #endregion

            #region Read

            internal void Read(Stream stream)
            {
                Int64 index = 0L;
                Byte[] magicCodeArray = new Byte[MagicCode.Length];

                Read(stream, magicCodeArray, ref index);
                Read(stream, ref VersionToken, ref index);
                Read(stream, ref CurrentOffset, ref index);
                Read(stream, ref PrevOffset, ref index);
                Read(stream, ref FileCount, ref index);
                Read(stream, ref ActiveTimeValue, ref index);
                Read(stream, ref FinalOffset, ref index);
                Read(stream, ref LastFileTimeValue, ref index);
                Read(stream, ref CycleTotalFileCount, ref index);
                Read(stream, ref OverwriteCount, ref index);
                Read(stream, ref OverwriteTimeValue, ref index);

                FeatureMatch = ArrayCompare(magicCodeArray, MagicCode);
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

            public static Byte[] Read(Stream stream, Int32 bufferSize, ref Int64 index)
            {
                Byte[] buffer = new Byte[bufferSize];
                Read(stream, buffer, ref index);

                return buffer;
            }

            public static void Read(Stream stream, Byte[] buffer, ref Int64 index)
            {
                stream.Seek(index, SeekOrigin.Begin);
                stream.Read(buffer, 0, buffer.Length);
                index += buffer.Length;
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
                Write(stream, FinalOffset, ref index);
                Write(stream, LastFileTimeValue, ref index);
                Write(stream, CycleTotalFileCount, ref index);
                Write(stream, OverwriteCount, ref index);
                Write(stream, OverwriteTimeValue, ref index);

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

            public static void Write(Stream stream, Byte value, ref Int64 index)
            {
                Byte[] buffer = new Byte[] { value };
                Write(stream, buffer, ref index);
            }

            public static void Write(Stream stream, Byte[] buffer, ref Int64 index)
            {
                stream.Seek(index, SeekOrigin.Begin);
                stream.Write(buffer, 0, buffer.Length);
                index += buffer.Length;
            }

            #endregion

            #region Help

            public static Boolean ArrayCompare(Byte[] a1, Byte[] a2)
            {
                if (a1 == a2)
                {
                    return true;
                }

                if (a1 == null || a2 == null)
                {
                    return false;
                }

                if (a1.Length != a2.Length)
                {
                    return false;
                }

                for (Int32 i = 0; i < a1.Length; i++)
                {
                    if (a1[i] != a2[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public static DateTime ToDateTime(Int64 value)
            {
                const Int64 MIN_VALUE = 0L;
                const Int64 MAX_VALUE = 315537897599999L;

                if (value < MIN_VALUE || value > MAX_VALUE)
                {
                    return StartTime;
                }

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
            lock (String.Intern(_filePath))
            {
                _stream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                if (!_headerDict.ContainsKey(_filePath))
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

            if (buffer.Length == 0)
            {
                return 0L;
            }

            Int64 result = 0L;
            Int64 index = 0L;
            Byte[] lengthArray = BitConverter.GetBytes(buffer.Length);
            Byte[] timeArray = BitConverter.GetBytes(Header.ToInt64(DateTime.Now));
            Byte lengthChecksum = Checksum(lengthArray);
            Byte dataChecksum = Checksum(buffer);
            Byte timeChecksum = Checksum(timeArray);
            Int64 dataLength = buffer.Length + Header.BLOCK_HEADER_SIZE;
            Int64 prevOffset = 0;
            Int64 nextOffset = SetOffset(dataLength, DateTime.Now, out prevOffset);

            Byte[] prevOffsetArray = BitConverter.GetBytes(prevOffset);
            Byte prevOffsetChecksum = Checksum(prevOffsetArray);
            Byte[] nextOffsetArray = BitConverter.GetBytes(nextOffset);
            Byte nextOffsetChecksum = Checksum(nextOffsetArray);

            result = nextOffset - dataLength;
            index = result;
            Byte[] dataPositionArray = BitConverter.GetBytes(result + Header.BLOCK_HEADER_SIZE);
            Byte dataPositionChecksum = Checksum(dataPositionArray);
            
            Header.Write(_stream, Header.MagicCode, ref index);
            Header.Write(_stream, lengthArray, ref index);
            Header.Write(_stream, lengthChecksum, ref index);
            Header.Write(_stream, dataChecksum, ref index);

            Header.Write(_stream, timeArray, ref index);
            Header.Write(_stream, timeChecksum, ref index);

            Header.Write(_stream, dataPositionArray, ref index);
            Header.Write(_stream, dataPositionChecksum, ref index);
            Header.Write(_stream, prevOffsetArray, ref index);
            Header.Write(_stream, prevOffsetChecksum, ref index);
            Header.Write(_stream, nextOffsetArray, ref index);
            Header.Write(_stream, nextOffsetChecksum, ref index);

            index = result + Header.BLOCK_HEADER_SIZE;
            Header.Write(_stream, buffer, ref index);

            _stream.Flush();

            return result;
        }

        public Byte[] Read(Int64 offset, out DateTime uploadDate)
        {
            Byte[] emptyResult = new Byte[0];
            Int64 index = offset;
            uploadDate = DateTime.Now;
            if (offset >= _length || offset == 0L)
            {
                return emptyResult;
            }

            Byte[] magicCode = Header.Read(_stream, Header.MagicCode.Length, ref index);

            if (!Header.ArrayCompare(magicCode, Header.MagicCode))
            {
                return emptyResult;
            }

            Byte[] lengthArray = Header.Read(_stream, Header.Int32_SIZE, ref index);
            Byte[] lengthChecksum = Header.Read(_stream, 1, ref index);
            Byte[] dataChecksum = Header.Read(_stream, 1, ref index);

            Byte[] timeArray = Header.Read(_stream, Header.Int64_SIZE, ref index);
            Byte[] timeChecksum = Header.Read(_stream, 1, ref index);

            Int32 dataLength = BitConverter.ToInt32(lengthArray, 0);

            if (lengthChecksum[0] != Checksum(lengthArray))
            {
                return emptyResult;
            }

            if (timeArray[0] == Checksum(timeArray))
            {
                Int64 timeValue = BitConverter.ToInt64(timeArray, 0);
                uploadDate = Header.ToDateTime(timeValue);
            }

            index = offset + Header.BLOCK_HEADER_SIZE;
            Byte[] result = Header.Read(_stream, dataLength, ref index);

            if (dataChecksum[0] != Checksum(result))
            {
                return emptyResult;
            }

            return result;
        }

        public Header GetHeader()
        {
            Header header = _headerDict[_filePath];
            header.Read(_stream);
            header.FreeStorage = _length - header.CurrentOffset;

            return header;
        }

        private Int64 SetOffset(Int64 dataSize, DateTime time, out Int64 prevOffset)
        {
            lock (String.Intern(_filePath))
            {
                Header header = GetHeader();
                prevOffset = header.PrevOffset;
                Int64 @newOffset = header.CurrentOffset + dataSize;

                if (@newOffset > _length)
                {
                    @newOffset = Header.OFFSET_SIZE + dataSize;
                    header.FinalOffset = header.PrevOffset;
                    header.FinalFileTime = header.ActiveTime;
                    header.OverwriteCount = header.OverwriteCount + 1;
                    if (header.OverwriteCount > _length)
                    {
                        header.OverwriteCount = 0;
                    }
                    header.OverwriteTime = DateTime.Now;
                    header.CycleTotalFileCount = header.FileCount;
                    header.FileCount = 0;
                }

                if (@newOffset < Header.OFFSET_SIZE)
                {
                    @newOffset = Header.OFFSET_SIZE + dataSize;
                    header.FinalOffset = 0;
                    header.FinalFileTime = DateTime.MinValue;
                    header.OverwriteCount = 0;
                    header.OverwriteTime = DateTime.MinValue;
                    header.CycleTotalFileCount = 0;
                    header.FileCount = 0;
                }

                header.FileCount = header.FileCount + 1;
                header.PrevOffset = header.CurrentOffset;
                header.CurrentOffset = @newOffset;
                header.ActiveTime = time;
                header.FreeStorage = _length - header.CurrentOffset;

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

        ~BigFile()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (IsDisposed || !disposing)
            {
                return;
            }

            if (_stream != null)
            {
                _stream.Dispose();
            }

            IsDisposed = true;
        }
    }
}
