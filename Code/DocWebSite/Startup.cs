using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DocWebSite.Startup))]
namespace DocWebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
