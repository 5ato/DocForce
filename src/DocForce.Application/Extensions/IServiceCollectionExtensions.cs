using DocForce.Application.Behaviors;
using DocForce.Application.Common.Dispatcher;
using DocForce.Application.Common.Dispatcher.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Frozen;
using System.Reflection;

namespace DocForce.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddDispatcher(assembly);

        services.AddBehaviors();

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
    }

    internal static void AddBehaviors(this IServiceCollection services)
    {
        services.AddPipelineBehavior(typeof(LoggingBehavior<,>));
        services.AddPipelineBehavior(typeof(ValidationBehavior<,>));
    }

    internal static IServiceCollection AddDispatcher(this IServiceCollection services, Assembly assembly)
    {
        var requestWrappers = new Dictionary<Type, RequestHandlerBase>();
        var domainEventWrappers = new Dictionary<Type, DomainEventHandlerBase>();

        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface) continue;

            foreach (var iface in type.GetInterfaces())
            {
                if (!iface.IsGenericType) continue;
                var def = iface.GetGenericTypeDefinition();

                if (def == typeof(IRequestHandler<,>))
                {
                    services.AddScoped(iface, type);

                    var args = iface.GetGenericArguments();
                    var requestType = args[0];
                    var responseType = args[1];

                    if (!requestWrappers.ContainsKey(requestType))
                    {
                        var wrapperType = typeof(RequestHandlerWrapper<,>)
                            .MakeGenericType(requestType, responseType);
                        requestWrappers[requestType] = (RequestHandlerBase)Activator.CreateInstance(wrapperType)!;
                    }
                }
                else if (def == typeof(IDomainEventHandler<>))
                {
                    services.AddScoped(iface, type);

                    var domainEventType = iface.GetGenericArguments()[0];

                    if (!domainEventWrappers.ContainsKey(domainEventType))
                    {
                        var wrapperType = typeof(DomainEventHandlerWrapper<>)
                            .MakeGenericType(domainEventType);
                        domainEventWrappers[domainEventType] = (DomainEventHandlerBase)Activator.CreateInstance(wrapperType)!;
                    }
                }
            }
        }

        var registry = new DispatcherRegistry(
            requestWrappers.ToFrozenDictionary(),
            domainEventWrappers.ToFrozenDictionary());

        services.AddSingleton(registry);
        services.AddScoped<Dispatcher>();
        services.AddScoped<ISender>(sp => sp.GetRequiredService<Dispatcher>());
        services.AddScoped<IPublisher>(sp => sp.GetRequiredService<Dispatcher>());

        return services;
    }

    public static IServiceCollection AddPipelineBehavior(
        this IServiceCollection services,
        Type openGenericBehaviorType)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), openGenericBehaviorType);
        return services;
    }
}
