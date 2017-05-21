using Lab.Livraria.Aplicacao;
using Lab.Livraria.Infra;
using Lab.Livraria.Models.ORM;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Lab.Livraria
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new Container();
            container.Register<DbContext, LivrariaContexto>();
            container.Register<ILivroAplicacao, LivroAplicacao>();

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}
