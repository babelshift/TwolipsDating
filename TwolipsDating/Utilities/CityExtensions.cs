using System;
using System.Text;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class CityExtensions
    {
        public static string ToFullLocationString(string cityName, string stateAbbreviation, string countryName = null)
        {
            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrEmpty(cityName))
            {
                if (cityName == "Unknown")
                {
                    return "Unknown";
                }

                if(String.IsNullOrEmpty(stateAbbreviation))
                {
                    stateAbbreviation = "Unknown";
                }

                if (countryName == "United States" || String.IsNullOrEmpty(countryName))
                {
                    sb.AppendFormat("{0}, {1}", cityName, stateAbbreviation);
                }
                else
                {
                    sb.AppendFormat("{0}, {1}, {2}", cityName, stateAbbreviation, countryName);
                }
            }

            return sb.ToString();
        }

        public static string ToFullLocationString(this GeoCity geoCity)
        {
            string cityName = geoCity != null ? geoCity.Name : String.Empty;
            string stateAbbreviation = geoCity != null && geoCity.GeoState != null ? geoCity.GeoState.Abbreviation : String.Empty;
            string countryName = geoCity != null && geoCity.GeoState != null && geoCity.GeoState.GeoCountry != null ? geoCity.GeoState.GeoCountry.Name : String.Empty;

            return ToFullLocationString(cityName, stateAbbreviation, countryName);
        }
    }
}