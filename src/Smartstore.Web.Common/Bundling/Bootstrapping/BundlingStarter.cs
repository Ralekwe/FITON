﻿using System;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Smartstore.Engine;
using Smartstore.Engine.Builders;
using Smartstore.IO;
using Smartstore.Web.Bundling;
using Smartstore.Web.Bundling.Processors;
using Smartstore.Web.Theming;
using WebOptimizer;

namespace Smartstore.Web.Bootstrapping
{
    internal class BundlingStarter : StarterBase
    {
        public BundlingStarter()
        {
            RunAfter<MvcStarter>();
        }

        private static IFileProvider ResolveThemeFileProvider(string themeName, IApplicationContext appContext)
        {
            var themeRegistry = appContext.Services.Resolve<IThemeRegistry>();
            return themeRegistry?.GetThemeManifest(themeName)?.WebFileProvider;
        }

        private static IFileProvider ResolveModuleFileProvider(string moduleName, IApplicationContext appContext)
        {
            return appContext.ModuleCatalog.GetModuleByName(moduleName, true)?.WebFileProvider;
        }

        public override void ConfigureServices(IServiceCollection services, IApplicationContext appContext, bool isActiveModule)
        {
            // Configure & register asset file provider
            var fileProvider = new AssetFileProvider(appContext.WebRoot);

            fileProvider.AddFileProvider("themes/", ResolveThemeFileProvider);
            fileProvider.AddFileProvider("modules/", ResolveModuleFileProvider);
            fileProvider.AddFileProvider(".app/", new SassFileProvider(appContext));

            services.AddSingleton<IAssetFileProvider>(fileProvider);

            // Configure bundling
            var isProduction = appContext.HostEnvironment.IsProduction();

            var cssBundlingSettings = new CssBundlingSettings 
            { 
                Minify = isProduction, 
                Concatenate = isProduction,
                AdjustRelativePaths = isProduction,
            };
            
            var jsBundlingSettings = new CodeBundlingSettings 
            { 
                Minify = isProduction,
                Concatenate = isProduction,
                AdjustRelativePaths = isProduction,
            };

            var codeSettings = jsBundlingSettings.CodeSettings;
            codeSettings.AmdSupport = true;
            codeSettings.IgnoreAllErrors = true;
            //codeSettings.ScriptVersion = ScriptVersion.EcmaScript6;
            codeSettings.MinifyCode = isProduction;
            codeSettings.IgnoreErrorCollection.Add("JS1010");

            var environment = (IWebHostEnvironment)appContext.HostEnvironment;
            var publisher = new BundlePublisher();

            services.AddWebOptimizer(environment, cssBundlingSettings, jsBundlingSettings, assetPipeline => 
            {
                publisher.RegisterBundles(appContext, assetPipeline);

                foreach (var asset in assetPipeline.Assets)
                {
                    if (asset.Items.ContainsKey("fileprovider") || asset.Items.ContainsKey("usecontentroot"))
                    {
                        // A custom file provider was added already, leave it alone.
                        continue;
                    }
                    
                    asset.UseFileProvider(fileProvider);
                }
            });

            services.AddNodeServices(o => 
            {
                // TODO: (core) Configure NodeServices?
            });

            services.AddTransient<IConfigureOptions<WebOptimizerOptions>, BundlingConfigurer>();
        }

        public override void ConfigureContainer(ContainerBuilder builder, IApplicationContext appContext, bool isActiveModule)
        {
            builder.RegisterDecorator<SmartAssetBuilder, IAssetBuilder>();
        }

        public override void BuildPipeline(RequestPipelineBuilder builder)
        {
            builder.Configure(StarterOrdering.BeforeStaticFilesMiddleware, app =>
            {
                //app.UseWebOptimizer();
                app.UseBundling();
            });

            builder.Configure(StarterOrdering.StaticFilesMiddleware, app =>
            {
                // TODO: (core) Set StaticFileOptions
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = builder.ApplicationBuilder.ApplicationServices.GetRequiredService<IAssetFileProvider>(),
                    ContentTypeProvider = MimeTypes.ContentTypeProvider
                });
            });
        }
    }
}
