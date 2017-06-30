using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json.Linq;

namespace TypeSafeSignalR.Common
{
    public static class ClientHubSubscriber<T> where T : class
    {
        public static void CreateSubs<K>(IHubProxy hubProxy, K imp) where K : T
        {
            foreach (var interfaceMethod in typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                string eventName = interfaceMethod.Name;
                var retTp = interfaceMethod.ReturnType;
                long paramCount = interfaceMethod.GetParameters().Length;

                Subscription subscription = hubProxy.Subscribe(eventName);

                var paramInfo = interfaceMethod.GetParameters().ToList();

                subscription.Received += delegate (IList<JToken> list)
                {
                    List<object> invocParams = new List<object>();
                    int idx = 0;
                    foreach (JToken jToken in list)
                    {
                        invocParams.Add(jToken.ToObject(paramInfo[idx].ParameterType));
                        ++idx;
                    }
                    interfaceMethod.Invoke(imp, invocParams.ToArray());
                };
            }
        }
    }
}
