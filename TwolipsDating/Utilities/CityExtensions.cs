using System.Text;
using TwolipsDating.Models;

namespace TwolipsDating.Utilities
{
    public static class CityExtensions
    {
        public static string ToFullLocationString(this GeoCity geoCity)
        {
            StringBuilder sb = new StringBuilder();

            if (geoCity != null)
            {
                if(geoCity.GeoState.GeoCountry.Name == "United States")
                {
                    sb.AppendFormat("{0}, {1}", geoCity.Name, geoCity.GeoState.Abbreviation);
                }
                else
                {
                    sb.AppendFormat("{0}, {1}, {2}", geoCity.Name, geoCity.GeoState.Abbreviation, geoCity.GeoState.GeoCountry.Name);
                }
            }

            return sb.ToString();
        }
    }
}