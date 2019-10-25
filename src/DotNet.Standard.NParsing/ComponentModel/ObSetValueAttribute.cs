using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotNet.Standard.NParsing.ComponentModel
{

    /*internal class ObSetValueAttribute : ProxyAttribute
    {
        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            var realProxy = new ObSetValueProxy(serverType);
            return realProxy.GetTransparentProxy() as MarshalByRefObject;
        }
    }

    internal class ObSetValueProxy : DispatchProxy
    {
        //private readonly Type _serverType;
        private readonly MethodBase _method;

        public ObSetValueProxy(Type serverType) : base(serverType)
        {
            //_serverType = serverType;
            _method = serverType.GetMethod("SetPropertyValid", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override IMessage Invoke(IMessage msg)
        {
            if (msg is IConstructionCallMessage) // 如果是构造函数，按原来的方式返回即可。
            {
                var constructCallMsg = msg as IConstructionCallMessage;
                var constructionReturnMessage = InitializeServerObject(constructCallMsg);
                SetStubData(this, constructionReturnMessage.ReturnValue);
                return constructionReturnMessage;
            }
            if (msg is IMethodCallMessage) //如果是方法调用（属性也是方法调用的一种）
            {
                var callMsg = msg as IMethodCallMessage;
                var args = callMsg.Args;
                IMessage message;
                try
                {
                    if (callMsg.MethodName.StartsWith("set_") && args.Length == 1)
                    {
                        if (!(args[0] is DateTime) || (((DateTime)args[0]).Year >= 1900 &&
                            ((DateTime)args[0]).Month > 0 && ((DateTime)args[0]).Day > 0))
                        {
                            /*var propertyInfo = 
                                (from p in _serverType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                    where p.GetGetMethod(true) == callMsg.MethodBase ||
                                          p.GetSetMethod(true) == callMsg.MethodBase
                                    select p).FirstOrDefault();#1#

                            //var m = GetUnwrappedServer().GetType().GetMethod("SetPropertyValid", BindingFlags.Instance | BindingFlags.Public);
                            //m.Invoke(GetUnwrappedServer(), new object[] { callMsg.MethodName.Substring(4) });
                            _method.Invoke(GetUnwrappedServer(), new object[] { callMsg.MethodName.Substring(4) });//对属性进行调用
                        }
                    }
                    var obj = callMsg.MethodBase.Invoke(GetUnwrappedServer(), args);
                    message = new ReturnMessage(obj, args, args.Length, callMsg.LogicalCallContext, callMsg);
                }
                catch (Exception e)
                {
                    message = new ReturnMessage(e, callMsg);
                }
                return message;
            }
            return msg;
        }
    }*/
}
