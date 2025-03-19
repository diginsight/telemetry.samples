using System.Web;
using System.Web.Mvc;

namespace S10_01_AspnetWebAPI480SdkProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
