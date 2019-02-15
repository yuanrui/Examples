namespace Simple.Common.Utility
{
    using System;
    using System.Security.Cryptography;

    public class SafeRandom
    {
        private static readonly RandomNumberGenerator GlobalGenerator = RandomNumberGenerator.Create();

        [ThreadStatic]
        private static Random _random;

        private static Random GetRandom()
        {
            if (_random == null)
            {
                Byte[] buffer = new Byte[4];
                GlobalGenerator.GetBytes(buffer);
                _random = new Random(BitConverter.ToInt32(buffer, 0));
            }

            return _random;
        }

        public Int32 Next()
        {
            return GetRandom().Next();
        }

        public Int32 Next(Int32 maxValue)
        {
            return GetRandom().Next(maxValue);
        }

        public Int32 Next(Int32 minValue, Int32 maxValue)
        {
            return GetRandom().Next(minValue, maxValue);
        }

        public void NextBytes(Byte[] buffer)
        {
            GetRandom().NextBytes(buffer);
        }

        public Double NextDouble()
        {
            return GetRandom().NextDouble();
        }
    }
}
