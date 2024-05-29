using System;
using System.Collections.Generic;
using UnityEngine;

namespace Service
{
    public static class ServiceProvider
    {
        static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service)
        {
            Type serviceType = typeof(T);
            if (_services.ContainsKey(serviceType))
            {
                Debug.LogWarning($"[Warning] Service {serviceType.Name} is already registered in the services map." +
                                 $" The old service will be overwritten");
            }

            _services[serviceType] = service;
        }

        public static T Get<T>()
        {
            if (!_services.TryGetValue(typeof(T), out object service))
                throw new Exception($"[ERROR] Service {typeof(T).Name} was not registered in the service locator");

            return (T)service;
        }

        public static void Unregister<T>()
        {
            if(_services.TryGetValue(typeof(T), out object service) && service is IDisposable disposableService)
            {
                disposableService.Dispose();
            }

            if(!_services.Remove(typeof(T)))
            {
                Debug.LogError($"[ERROR] Service {typeof(T).Name} was not registered in the service locator");
            }
        }
    }
}
