using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AppointLess2.Startup))]
namespace AppointLess2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
