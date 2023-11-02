using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LapProject.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToEuroString(this decimal num)
        {
            return $"{num.ToString("N2")} €";
        }
    }
}