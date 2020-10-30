using HybridModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IMvcBuilderExtensions
    {
        public static IMvcBuilder AddHybridModelBinder(this IMvcBuilder builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return builder.AddHybridModelBinder(_ => { });
        }

        public static IMvcBuilder AddHybridModelBinder(
            this IMvcBuilder builder,
            Action<HybridModelBinderOptions> setupAction)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services
                .Configure(setupAction)
                .ConfigureOptions<HybridModelBinderSetup>();

            return builder
                .AddMvcOptions(mvcOptions => mvcOptions.AddHybridModelBinder());
        }

        public static IMvcCoreBuilder AddHybridModelBinder(this IMvcCoreBuilder builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return builder.AddHybridModelBinder(_ => { });
        }

        public static IMvcCoreBuilder AddHybridModelBinder(
            this IMvcCoreBuilder builder,
            Action<HybridModelBinderOptions> setupAction)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services
                .Configure(setupAction)
                .ConfigureOptions<HybridModelBinderSetup>();

            return builder
                .AddMvcOptions(mvcOptions => mvcOptions.AddHybridModelBinder());
        }

        private static void AddHybridModelBinder(this MvcOptions mvcOptions)
        {
            var hybridConvention = new HybridModelBinderApplicationModelConvention();

            mvcOptions.Conventions.Add(hybridConvention);
        }

        private class HybridModelBinderSetup : IPostConfigureOptions<MvcOptions>
        {
            public HybridModelBinderSetup(
                IOptions<HybridModelBinderOptions> binderOptions,
                IHttpRequestStreamReaderFactory readerFactory)
            {
                this.binderOptions = binderOptions.Value;
                this.readerFactory = readerFactory;
            }

            private readonly HybridModelBinderOptions binderOptions;
            private readonly IHttpRequestStreamReaderFactory readerFactory;

            public void PostConfigure(string name, MvcOptions options)
            {
                var provider = !binderOptions.Passthrough
                ? (IModelBinderProvider)new DefaultHybridModelBinderProvider(options.InputFormatters, readerFactory, binderOptions.FallbackBindingOrder)
                : new DefaultPassthroughHybridModelBinderProvider(options.InputFormatters, readerFactory, binderOptions.FallbackBindingOrder);

                options.ModelBinderProviders.Insert(0, provider);
            }
        }
    }
}