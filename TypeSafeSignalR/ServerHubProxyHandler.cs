using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Microsoft.AspNet.SignalR.Client;

namespace StreamCentral.Common
{
    public class ServerHubProxyHandler<T> : RealProxy
    {
        private readonly IHubProxy _hubProxy;
        //private readonly MethodInfo _hubInvokableMethod;
        private ServerHubProxyHandler(IHubProxy hubProxy) : base(typeof(T))
        {
            this._hubProxy = hubProxy;
        }

        public static T Create(IHubProxy hubProxy)
        {
            return (T)new ServerHubProxyHandler<T>(hubProxy).GetTransparentProxy();
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = (IMethodCallMessage)msg;
            var method = (MethodInfo)methodCall.MethodBase;

            try
            {
                Console.WriteLine(method.Name);
                if (method.ReturnType != typeof(void))
                {
                    var t = method.ReturnType;//typeof(Task<>).MakeGenericType(new Type[] { method.ReturnType });
                    //var task = //_hubProxy.Invoke<object>(method.Name, methodCall.InArgs);
                    //task<T>

                    var _hubInvokableMethod = typeof(IHubProxy).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                            .Single(m => m.IsGenericMethod && m.ReturnType.IsGenericType && m.GetParameters().Count() == 2);

                    var signalRInvoked = (_hubInvokableMethod.MakeGenericMethod(t).Invoke(_hubProxy, new object[] { method.Name, methodCall.InArgs }));
                    //getting Task<T> result
                    var res = signalRInvoked.GetType().GetProperty("Result").GetValue(signalRInvoked);
                    return new ReturnMessage(res, null, 0, methodCall.LogicalCallContext, methodCall);
                }
                else
                {
                    var task = _hubProxy.Invoke(method.Name, methodCall.InArgs);
                    task.Wait();
                    return new ReturnMessage(null, null, 0, methodCall.LogicalCallContext, methodCall);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                if (e is TargetInvocationException && e.InnerException != null)
                {
                    return new ReturnMessage(e.InnerException, msg as IMethodCallMessage);
                }

                return new ReturnMessage(e, msg as IMethodCallMessage);
            }
        }
    }
}
