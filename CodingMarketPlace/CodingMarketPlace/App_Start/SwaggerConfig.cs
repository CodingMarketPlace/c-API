using System.Web.Http;
using WebActivatorEx;
using CodingMarketPlace;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace CodingMarketPlace
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            /*GlobalConfiguration.Configuration 
                .EnableSwagger(c =>
                    {
                        // Cette ligne de code permet de modifier l'accès à l'url des documents pour la génération du swagger
                        c.RootUrl(req => "http://codingmarketplace.apphb.com");
                        
                        c.SingleApiVersion("v1", "CodingMarketPlace");

                        c.IncludeXmlComments(GetXmlCommentsPath());
                    })
                .EnableSwaggerUi(c =>
                    {
                       
                    });*/
        }

        protected static string GetXmlCommentsPath()
        {
            return System.String.Format(@"{0}/bin/CodingMarketPlace.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
