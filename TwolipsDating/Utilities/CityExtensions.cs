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
        public static string ToFullLocationString(this GeoCity geoCity)
        {
            StringBuilder sb = new StringBuilder();

            if(geoCity != null)
            {
                sb.AppendFormat("{0}, {1}, {2}", geoCity.Name, geoCity.GeoState.Abbreviation, geoCity.GeoState.GeoCountry.Name);
            }

            return sb.ToString();
        }
    }
}