// Copyright (c) 2023 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Test.Performance
{
    public class Pbkdf2Test
    {
        static RandomNumberGenerator _rnGenerator = RandomNumberGenerator.Create();
        const string Password = "YR@2023666";

        private byte[] CreateSalt(int size)
        {
            var result = new byte[size];
            _rnGenerator.GetBytes(result);

            return result;
        }

        [Benchmark]
        public byte[] MD5_Salt32_Iterations1000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 1000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] MD5_Salt16_Iterations1000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 1000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA1_Salt32_Iterations1000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 1000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA1_Salt16_Iterations1000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 1000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA256_Salt32_Iterations1000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 1000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA256_Salt16_Iterations1000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 1000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA384_Salt32_Iterations1000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 1000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA384_Salt16_Iterations1000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 1000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA512_Salt32_Iterations1000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 1000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA512_Salt16_Iterations1000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 1000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] MD5_Salt32_Iterations2000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 2000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] MD5_Salt16_Iterations2000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 2000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA1_Salt32_Iterations2000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 2000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA1_Salt16_Iterations2000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 2000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA256_Salt32_Iterations2000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 2000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA256_Salt16_Iterations2000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 2000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA384_Salt32_Iterations2000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 2000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA384_Salt16_Iterations2000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 2000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA512_Salt32_Iterations2000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 2000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA512_Salt16_Iterations2000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 2000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] MD5_Salt32_Iterations3000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 3000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] MD5_Salt16_Iterations3000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 3000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA1_Salt32_Iterations3000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 3000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA1_Salt16_Iterations3000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 3000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA256_Salt32_Iterations3000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 3000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA256_Salt16_Iterations3000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 3000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA384_Salt32_Iterations3000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 3000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA384_Salt16_Iterations3000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 3000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA512_Salt32_Iterations3000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 3000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA512_Salt16_Iterations3000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 3000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] MD5_Salt32_Iterations5000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 5000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] MD5_Salt16_Iterations5000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 5000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA1_Salt32_Iterations5000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 5000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA1_Salt16_Iterations5000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 5000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA256_Salt32_Iterations5000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 5000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA256_Salt16_Iterations5000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 5000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA384_Salt32_Iterations5000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 5000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA384_Salt16_Iterations5000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 5000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA512_Salt32_Iterations5000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 5000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA512_Salt16_Iterations5000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 5000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] MD5_Salt32_Iterations10000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 10000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] MD5_Salt16_Iterations10000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 10000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA1_Salt32_Iterations10000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 10000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA1_Salt16_Iterations10000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 10000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA256_Salt32_Iterations10000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 10000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA256_Salt16_Iterations10000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 10000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA384_Salt32_Iterations10000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 10000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA384_Salt16_Iterations10000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 10000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA512_Salt32_Iterations10000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 10000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA512_Salt16_Iterations10000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 10000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] MD5_Salt32_Iterations20000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 20000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] MD5_Salt16_Iterations20000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 20000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA1_Salt32_Iterations20000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 20000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA1_Salt16_Iterations20000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 20000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA256_Salt32_Iterations20000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 20000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA256_Salt16_Iterations20000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 20000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA384_Salt32_Iterations20000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 20000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA384_Salt16_Iterations20000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 20000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA512_Salt32_Iterations20000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 20000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA512_Salt16_Iterations20000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 20000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] MD5_Salt32_Iterations30000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 30000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] MD5_Salt16_Iterations30000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 30000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA1_Salt32_Iterations30000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 30000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA1_Salt16_Iterations30000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 30000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA256_Salt32_Iterations30000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 30000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA256_Salt16_Iterations30000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 30000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA384_Salt32_Iterations30000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 30000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA384_Salt16_Iterations30000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 30000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA512_Salt32_Iterations30000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 30000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA512_Salt16_Iterations30000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 30000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] MD5_Salt32_Iterations50000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 50000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] MD5_Salt16_Iterations50000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 50000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA1_Salt32_Iterations50000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 50000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA1_Salt16_Iterations50000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 50000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA256_Salt32_Iterations50000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 50000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA256_Salt16_Iterations50000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 50000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA384_Salt32_Iterations50000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 50000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA384_Salt16_Iterations50000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 50000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA512_Salt32_Iterations50000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 50000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA512_Salt16_Iterations50000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 50000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] MD5_Salt32_Iterations100000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 100000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] MD5_Salt16_Iterations100000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 100000, HashAlgorithmName.MD5);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA1_Salt32_Iterations100000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 100000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA1_Salt16_Iterations100000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 100000, HashAlgorithmName.SHA1);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA256_Salt32_Iterations100000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 100000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA256_Salt16_Iterations100000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 100000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA384_Salt32_Iterations100000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 100000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA384_Salt16_Iterations100000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 100000, HashAlgorithmName.SHA384);
            return pbkdf2.GetBytes(32);
        }

        [Benchmark]
        public byte[] SHA512_Salt32_Iterations100000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(32), 100000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }
        [Benchmark]
        public byte[] SHA512_Salt16_Iterations100000()
        {
            var pbkdf2 = new Rfc2898DeriveBytes(Password, CreateSalt(16), 100000, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(32);
        }

    }
}
