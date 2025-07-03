// Copyright (c) 2017 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data
{
    public interface IAutoCloseable : IDisposable
    {
        new void Dispose();
    }
}
