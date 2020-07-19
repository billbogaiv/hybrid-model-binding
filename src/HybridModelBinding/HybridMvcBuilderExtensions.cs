using HybridModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HybridMvcCoreBuilderExtensions
    {
        public static IMvcBuilder AddHybridModelBinder(this IMvcBuilder builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return builder.AddMvcOptions(mvcOptions => mvcOptions
                .AddHybridModelBinder(builder.Services));
        }

        public static IMvcBuilder AddHybridModelBinder(
            this IMvcBuilder builder,
            Action<HybridModelBinderOptions> setupAction)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return builder.AddMvcOptions(mvcOptions => mvcOptions
                .AddHybridModelBinder(builder.Services, setupAction));
        }

        public static IMvcCoreBuilder AddHybridModelBinder(this IMvcCoreBuilder builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return builder.AddMvcOptions(mvcOptions => mvcOptions
                .AddHybridModelBinder(builder.Services));
        }

        public static IMvcCoreBuilder AddHybridModelBinder(
            this IMvcCoreBuilder builder,
            Action<HybridModelBinderOptions> setupAction)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            return builder.AddMvcOptions(mvcOptions => mvcOptions
                .AddHybridModelBinder(builder.Services, setupAction));
        }

        private static void AddHybridModelBinder(
            this MvcOptions mvcOptions,
            IServiceCollection services,
            Action<HybridModelBinderOptions> setupAction = null)
        {
            var options = new HybridModelBinderOptions();
            setupAction?.Invoke(options);

            var serviceProvider = services.BuildServiceProvider();
            var readerFactory = serviceProvider.GetRequiredService<IHttpRequestStreamReaderFactory>();

            var hybridConvention = new HybridModelBinderApplicationModelConvention();
            mvcOptions.Conventions.Add(hybridConvention);

            var provider = !options.Passthrough
                ? (IModelBinderProvider)new DefaultHybridModelBinderProvider(mvcOptions.InputFormatters, readerFactory, options.FallbackBindingOrder)
                : new DefaultPassthroughHybridModelBinderProvider(mvcOptions.InputFormatters, readerFactory, options.FallbackBindingOrder);

            mvcOptions.ModelBinderProviders.Insert(0, provider);
        }
    }
}