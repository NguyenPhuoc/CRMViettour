using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CRMViettour.Startup))]
namespace CRMViettour
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
