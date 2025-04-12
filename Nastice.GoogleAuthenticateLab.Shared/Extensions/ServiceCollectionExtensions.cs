using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Nastice.GoogleAuthenticateLab.Shared.Interceptors;
using Serilog;

namespace Nastice.GoogleAuthenticateLab.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection addProxied<TImplementation>(this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        Func<IServiceProvider, TImplementation>? factory = null) where TImplementation : class
    {
        services.Add(new ServiceDescriptor(typeof(TImplementation),
                                           provider => {
                                               var proxyGenerator = new ProxyGenerator();

                                               // 如果提供了工廠方法，優先使用工廠方法生成實例
                                               if (factory != null)
                                               {
                                                   var target = factory(provider);
                                                   return proxyGenerator.CreateClassProxyWithTarget(target, new TraceLogInterceptor(Log.Logger));
                                               }

                                               // 取得建構的內容
                                               var constructor = typeof(TImplementation).GetConstructors()
                                                                                        .FirstOrDefault(x => x.GetParameters()
                                                                                                .Length >
                                                                                            0);

                                               // 如果不用沒有建構式或建構式不需要注入內容
                                               if (constructor is null)
                                               {
                                                   // 創建代理物件，應用攔截器
                                                   return proxyGenerator.CreateClassProxyWithTarget(
                                                   ActivatorUtilities.CreateInstance<TImplementation>(provider),
                                                   new TraceLogInterceptor(Log.Logger));
                                               }

                                               var parameters = constructor.GetParameters()
                                                                           .Select(p => provider.GetService(p.ParameterType))
                                                                           .ToArray();

                                               // 生成實際的服務實例
                                               var targetWithParameters = ActivatorUtilities.CreateInstance<TImplementation>(provider);

                                               // 創建代理物件，應用攔截器
                                               return proxyGenerator.CreateClassProxyWithTarget(
                                               typeof(TImplementation),
                                               targetWithParameters,
                                               new(),
                                               parameters,
                                               new TraceLogInterceptor(Log.Logger));
                                           },
                                           serviceLifetime));

        return services;
    }

    private static IServiceCollection addProxied<TInterface, TImplementation>(this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        Func<IServiceProvider, TImplementation>? factory = null) where TInterface : class where TImplementation : class, TInterface
    {
        services.Add(new ServiceDescriptor(typeof(TInterface),
                                           provider => {
                                               var proxyGenerator = new ProxyGenerator();

                                               // 生成實際的服務實例
                                               var target = factory is not null
                                                   ? factory(provider)
                                                   : ActivatorUtilities.CreateInstance<TImplementation>(provider);

                                               // 創建代理物件，應用攔截器
                                               return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(
                                               target,
                                               new TraceLogInterceptor(Log.Logger));
                                           },
                                           serviceLifetime));

        return services;
    }

    private static IServiceCollection addProxiedKeyed<TInterface, TImplementation>(this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        object? serviceKey,
        Func<IServiceProvider, object?, object>? factory = null) where TInterface : class where TImplementation : class, TInterface
    {
        var descriptor = new ServiceDescriptor(typeof(TInterface),
                                               serviceKey,
                                               (provider, test) => {
                                                   var proxyGenerator = new ProxyGenerator();

                                                   // 生成實際的服務實例
                                                   var target = factory is not null
                                                       ? factory(provider, test)
                                                       : ActivatorUtilities.CreateInstance<TImplementation>(provider);

                                                   // 創建代理物件，應用攔截器
                                                   return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(
                                                   (target as TImplementation)!,
                                                   new TraceLogInterceptor(Log.Logger));
                                               },
                                               serviceLifetime);

        services.Add(descriptor);

        return services;
    }


    public static IServiceCollection AddProxiedScoped<TImplementation>(this IServiceCollection services,
        Func<IServiceProvider, TImplementation>? factory = null) where TImplementation : class
    {
        return services.addProxied(ServiceLifetime.Scoped, factory);
    }

    public static IServiceCollection AddProxiedScoped<TInterface, TImplementation>(this IServiceCollection services,
        Func<IServiceProvider, TImplementation>? factory = null) where TInterface : class where TImplementation : class, TInterface
    {
        return services.addProxied<TInterface, TImplementation>(ServiceLifetime.Scoped, factory);
    }

    public static IServiceCollection AddProxiedKeyedScoped<TInterface, TImplementation>(this IServiceCollection services,
        string name,
        Func<IServiceProvider, object?, object>? factory = null) where TInterface : class where TImplementation : class, TInterface
    {
        return services.addProxiedKeyed<TInterface, TImplementation>(ServiceLifetime.Scoped, name, factory);
    }

    public static IServiceCollection AddProxiedTransient<TImplementation>(this IServiceCollection services,
        Func<IServiceProvider, TImplementation>? factory = null) where TImplementation : class
    {
        return services.addProxied(ServiceLifetime.Transient, factory);
    }

    public static IServiceCollection AddProxiedTransient<TInterface, TImplementation>(this IServiceCollection services,
        Func<IServiceProvider, TImplementation>? factory = null) where TInterface : class where TImplementation : class, TInterface
    {
        return services.addProxied<TInterface, TImplementation>(ServiceLifetime.Transient, factory);
    }

    public static IServiceCollection AddProxiedSingleton<TImplementation>(this IServiceCollection services,
        Func<IServiceProvider, TImplementation>? factory = null) where TImplementation : class
    {
        return services.addProxied(ServiceLifetime.Singleton, factory);
    }

    public static IServiceCollection AddProxiedSingleton<TInterface, TImplementation>(this IServiceCollection services,
        Func<IServiceProvider, TImplementation>? factory = null) where TInterface : class where TImplementation : class, TInterface
    {
        return services.addProxied<TInterface, TImplementation>(ServiceLifetime.Singleton, factory);
    }
}
