using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bi.Web.Startup))]
namespace Bi.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
