using System.Web;
using System.Web.Mvc;

namespace S10_00_AspnetWebApi480
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
