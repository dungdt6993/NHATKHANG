﻿using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class ItemsTypeVM : ItemsType
    {
        public string ITypeCode { get; set; }
        public string ITypeName { get; set; }
        public string ITypeDesc { get; set; }
        public bool IsInventory { get; set; }
    }
}
