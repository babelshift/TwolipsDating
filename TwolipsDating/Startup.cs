using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TwolipsDating.Startup))]

namespace TwolipsDating
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}