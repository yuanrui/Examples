// Copyright (c) 2021 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.MySql
{
    public class VehiclePassModel
    {
        public Int64 Id { get; set; }

        public string PlateNO { get; set; }

        public string PlateColor { get; set; }

        public DateTime PassTime { get; set; }

        public string EquipId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
