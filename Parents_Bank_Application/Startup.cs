using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Parents_Bank_Application.Startup))]
namespace Parents_Bank_Application
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
