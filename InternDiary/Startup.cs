using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InternDiary.Startup))]
namespace InternDiary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
