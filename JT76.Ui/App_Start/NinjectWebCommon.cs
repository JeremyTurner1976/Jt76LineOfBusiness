using System;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using JT76.Common.Services;
using JT76.Data.Database.ModelRepositories;
using JT76.Ui;
using JT76.Ui.ViewModels;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using WebActivatorEx;
using WebApiContrib.IoC.Ninject;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof (NinjectWebCommon), "Start")]
[assembly: ApplicationShutdownMethod(typeof (NinjectWebCommon), "Stop")]

namespace JT76.Ui
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        ///     Starts the application
        /// </summary>
        public static void Start()
        {
            Debug.WriteLine("NinjectWebCommon.Start()");

            DynamicModuleUtility.RegisterModule(typeof (OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof (NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        ///     Stops the application.
        /// </summary>
        public static void Stop()
        {
            Debug.WriteLine("NinjectWebCommon.Stop()");

            Bootstrapper.ShutDown();
        }

        /// <summary>
        ///     Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            Debug.WriteLine("NinjectWebCommon.CreateKernel()");

            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                //ADD this for web contributions ioc API ninject
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        ///     Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            Debug.WriteLine("NinjectWebCommon.RegisterServices()");

            //Implement the specific injectors for Data and the view models
            kernel.Bind<IErrorRepository>().To<ErrorRepository>().InRequestScope();
            kernel.Bind<ILogMessageRepository>().To<LogMessageRepository>().InRequestScope();
            kernel.Bind<AdminViewModel>().To<AdminViewModel>().InRequestScope();

            kernel.Bind<IReplyRepository>().To<ReplyRepository>().InRequestScope();
            kernel.Bind<ITopicRepository>().To<TopicRepository>().InRequestScope();
            kernel.Bind<MessageBoardViewModel>().To<MessageBoardViewModel>().InRequestScope();

            //setup services that will be needed for classes in common
            kernel.Bind<IConfigService>().To<ConfigService>().InRequestScope();
            kernel.Bind<IEmailService>().To<ConfigEmailService>().InRequestScope();
            kernel.Bind<IFileService>().To<FileService>().InRequestScope();

            //setup services that will be used locally from common
            kernel.Bind<EmailLoggingService>().To<EmailLoggingService>().InRequestScope();
            kernel.Bind<DbLoggingService>().To<DbLoggingService>().InRequestScope();
            kernel.Bind<FileLoggingService>().To<FileLoggingService>().InRequestScope();

            //setup the local services and services that will use common
            kernel.Bind<IUiService>().To<UiService>().InRequestScope();
        }
    }
}