using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LapProject.Extensions;

namespace LapProject.Data
{
    public partial class OrderLine
    {
        public decimal GrossUnitPrice { get { return Math.Round(NetUnitPrice * ((100 + TaxRate) / 100), 2); } }
        public decimal GrossLinePrice { get { return Math.Round(GrossUnitPrice * Amount, 2); } }
    }
}