using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Flashcards.Startup))]
namespace Flashcards
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
