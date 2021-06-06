using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Webagency.Startup))]
namespace Webagency
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
