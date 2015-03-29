using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class CityExtensions
    {
        public static string GetCityAndState(this City city)
        {
            StringBuilder sb = new StringBuilder();

            if(city != null)
            {
                sb.Append(city.Name);
            }

            if (city.USState != null)
            {
                sb.AppendFormat(", {0}", city.USState.Abbreviation);
            }

            return sb.ToString();
        }
    }
}