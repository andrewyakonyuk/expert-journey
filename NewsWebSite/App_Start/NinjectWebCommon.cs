[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NewsWebSite.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NewsWebSite.App_Start.NinjectWebCommon), "Stop")]

namespace NewsWebSite.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Models;
    using NHibernate;
    using NHibernate.Tool.hbm2ddl;
    using NHibernate.Cfg;
    using Models.Repository;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IRepository>().To<NHibernateRepository>();
            kernel.Bind<ISessionFactory>().ToMethod(context =>
            {
                var configuration = new Configuration();

                configuration.Configure();
                configuration.AddAssembly(typeof(Article).Assembly);

                ISessionFactory sessionFactory = configuration.BuildSessionFactory();

                //Позволяет Nhibernate самому создавать в БД таблицу и поля к ним. 
                new SchemaUpdate(configuration).Execute(true, true);

                return sessionFactory;
            }).InSingletonScope();
            kernel.Bind<IUserRepository>().To<UserDateBaseRepository>();
        }
    }
}
