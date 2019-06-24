using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookingConfirm.Startup))]
namespace BookingConfirm
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
