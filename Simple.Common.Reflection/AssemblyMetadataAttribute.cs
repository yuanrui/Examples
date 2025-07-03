// Copyright (c) 2022 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
//[assembly: AssemblyMetadata("Serviceable", "True")]

#if NETFRAMEWORK

namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    internal sealed class AssemblyMetadataAttribute : Attribute
    {
        public AssemblyMetadataAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}

#endif