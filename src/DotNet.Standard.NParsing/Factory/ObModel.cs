/**
 * 作    者：朱晓春(zhi_dian@163.com)
 * 创建时间：2019-11-02 09:47:56
 * 版 本 号：1.0.5
 * 功能说明：使用动态代理，简化模型类和条件类
 * --------------------------------------------------
 * 修改标识：增加方法
 * 修 改 人：朱晓春(zhi_dian@163.com)
 * 日    期：2022-03-15 14:38:00
 * 版 本 号：2.0.3
 * 修改内容：新增判断对象属性是否被赋值扩展方法
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using DotNet.Standard.Common.Utilities;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.Factory
{
    public static class ObModel
    {
        private static Dictionary<Type, Type> _dicProxyTypes = new Dictionary<Type, Type>();
        private const string DynamicAssemblyName = "DynamicAssembly";//动态程序集名称
        private const string DynamicModuleName = "DynamicAssemblyModule";
        private const string ProxyClassNameFormater = "{0}Proxy";
        private const MethodAttributes GetSetMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Virtual | MethodAttributes.HideBySig;

        public static T Create<T>(params object[] args) where T : ObModelBase
        {
            var proxyType = typeof(T);
            return (T)Create(proxyType, args);
        }

        public static object Create(Type proxyType, params object[] args)
        {
            if (_dicProxyTypes.ContainsKey(proxyType))
            {
                return Activator.CreateInstance(_dicProxyTypes[proxyType], args);
            }
            var assemblyName = new AssemblyName(DynamicAssemblyName);
            var assemblyBuilderAccess = AssemblyBuilderAccess.RunAndCollect; //必须要设置可回收，否则内存暴涨
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, assemblyBuilderAccess);
            //动态创建模块
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(DynamicModuleName);
            var proxyClassName = string.Format(ProxyClassNameFormater, proxyType.Name);
            //动态创建类代理
            var typeBuilderProxy = moduleBuilder.DefineType(proxyClassName, TypeAttributes.Public | TypeAttributes.Serializable | TypeAttributes.Class | TypeAttributes.AutoClass, proxyType);
            var constructorArgs = args.Select(o => o.GetType()).ToArray();
            var methodProxySet = proxyType.GetMethod("ProxySet", BindingFlags.Instance | BindingFlags.NonPublic);
            var constructorBuilder = typeBuilderProxy.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArgs);
            var ilgCtor = constructorBuilder.GetILGenerator();
            if (methodProxySet != null && args.Length > 0)
            {
                ilgCtor.Emit(OpCodes.Ldarg_0);
                for (var i = 0; i < args.Length; i++)
                {
                    ilgCtor.Emit(OpCodes.Ldarg_S, i + 1);
                }
                ilgCtor.Emit(OpCodes.Call, methodProxySet);
            }
            ilgCtor.Emit(OpCodes.Ret);//返回

            //获取被代理对象的所有属性,循环属性进行重写
            var properties = proxyType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var propertyName = propertyInfo.Name;
                var propertyType = propertyInfo.PropertyType;
                if (propertyType.IsSystem())
                {
                    //动态创建字段和属性
                    var fieldBuilder = typeBuilderProxy.DefineField("_" + propertyName.ToLower(), propertyType, FieldAttributes.Private);
                    var propertyBuilder = typeBuilderProxy.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
                    //重写属性的Get Set方法
                    var methodGet = typeBuilderProxy.DefineMethod("get_" + propertyName, GetSetMethodAttributes, propertyType, Type.EmptyTypes);
                    var methodSet = typeBuilderProxy.DefineMethod("set_" + propertyName, GetSetMethodAttributes, null, new Type[] { propertyType });

                    //il of get method
                    var ilGetMethod = methodGet.GetILGenerator();
                    ilGetMethod.Emit(OpCodes.Ldarg_0);   //this
                    ilGetMethod.Emit(OpCodes.Ldfld, fieldBuilder); //numA
                    ilGetMethod.Emit(OpCodes.Ret); //return numA
                    //il of set method
                    var ilSetMethod = methodSet.GetILGenerator();
                    ilSetMethod.Emit(OpCodes.Ldarg_0);  //this
                    ilSetMethod.Emit(OpCodes.Ldarg_1);  //value
                    ilSetMethod.Emit(OpCodes.Stfld, fieldBuilder); //numA = value;
                    var setPropertyValid = proxyType.GetMethod("SetPropertyValid", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                    if (setPropertyValid != null)
                    {
                        ilSetMethod.Emit(OpCodes.Ldarg_0);
                        ilSetMethod.Emit(OpCodes.Ldarg_0);
                        ilSetMethod.Emit(OpCodes.Ldstr, propertyName);
                        ilSetMethod.Emit(OpCodes.Call, setPropertyValid);
                        ilSetMethod.Emit(OpCodes.Pop);
                    }
                    ilSetMethod.Emit(OpCodes.Ret);   //return;

                    //设置属性的Get Set方法
                    propertyBuilder.SetGetMethod(methodGet);
                    propertyBuilder.SetSetMethod(methodSet);
                }
                /*else if (propertyType.Contains(typeof(ObModelBase)))
                {
                    //动态创建字段和属性
                    var fieldBuilder = typeBuilderProxy.DefineField("_" + propertyName.ToLower(), propertyType, FieldAttributes.Private);
                    var propertyBuilder = typeBuilderProxy.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
                    //重写属性的Get Set方法
                    var methodGet = typeBuilderProxy.DefineMethod("get_" + propertyName, GetSetMethodAttributes, propertyType, Type.EmptyTypes);
                    var methodSet = typeBuilderProxy.DefineMethod("set_" + propertyName, GetSetMethodAttributes, null, new Type[] { propertyType });

                    //il of get method
                    var ilGetMethod = methodGet.GetILGenerator();
                    ilGetMethod.Emit(OpCodes.Newobj, propertyType.GetConstructor(new Type[0]));
                    ilGetMethod.Emit(OpCodes.Call, typeof(ObTerm).GetMethod("CreateProxyByEmit"));
                    ilGetMethod.Emit(OpCodes.Ret);

                    //il of set method
                    var ilSetMethod = methodSet.GetILGenerator();
                    ilSetMethod.Emit(OpCodes.Ldarg_0);  //this
                    ilSetMethod.Emit(OpCodes.Ldarg_1);  //value
                    ilSetMethod.Emit(OpCodes.Stfld, fieldBuilder); //numA = value;

                    //设置属性的Get 
                    propertyBuilder.SetGetMethod(methodGet);
                    propertyBuilder.SetSetMethod(methodSet);
                }*/
            }

            //使用动态类创建类型
            var proxyClassType = typeBuilderProxy.CreateTypeInfo();
            //创建类实例
            var instance = Activator.CreateInstance(proxyClassType, args);
            lock (_dicProxyTypes)
            {
                if (_dicProxyTypes.ContainsKey(proxyType))
                {
                    _dicProxyTypes.Add(proxyType, proxyClassType);
                }
            }
            //proxyClassType = null;
            //properties = null;
            //ilgCtor = null;
            //constructorBuilder = null;
            //methodProxySet = null;
            //typeBuilderProxy = null;
            //moduleBuilder = null;
            //assemblyBuilder = null;
            ////assemblyBuilderAccess = null;
            //assemblyName = null;
            return instance;
        }

        public static TSource Of<TSource>(this TSource source) where TSource : ObModelBase
        {
            return Create<TSource>(source);
        }

        public static object CreateProxyByEmit(ObModelBase source)
        {
            return Create(source.GetType(), source);
        }

        /// <summary>
        /// 判断对象属性是否被赋值
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="model"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static bool IsPropertyValid<TModel, TKey>(this TModel model, Expression<Func<TModel, TKey>> keySelector)
            where TModel : ObModelBase
        {
            if (!(keySelector.Body is MemberExpression meExp))
            {
                throw new Exception("只支持对象属性表达式");
            }
            var ret = GetSubModel(model, keySelector.Body, out var subModel);
            switch (ret)
            {
                case 2:
                    return subModel.IsPropertyValid(meExp.Member.Name);
                case 3:
                    return subModel != null;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 获取值对象
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="expression"></param>
        /// <param name="subModel"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static int GetSubModel<TModel>(TModel model, Expression expression, out ObModelBase subModel, int index = -1)
            where TModel : ObModelBase
        {
            index++;
            if (!(expression is MemberExpression meExp))
            {
                subModel = model;
                return 2;
            }
            var m = GetSubModel(model, meExp.Expression, out subModel, index);
            if (subModel == null)
            {
                //subModel = null;
                return m;
            }
            var property = subModel.GetType().GetProperty(meExp.Member.Name);
            var obj = property?.GetValue(subModel);
            if (index == 0)
            {
                if (property != null && typeof(ObModelBase).IsAssignableFrom(property.PropertyType))
                {
                    subModel = (ObModelBase)obj;
                    return 3;
                }
                //subModel = m;
                //递归到最后属性，返回父类对象
                return 2;
            }
            if (obj is ObModelBase om)
            {
                subModel = om;
                return 2;
            }
            subModel = null;
            return 1;
        }
    }

    public static class ObTerm
    {
        private const string DynamicAssemblyName = "DynamicAssembly";//动态程序集名称
        private const string DynamicModuleName = "DynamicAssemblyModule";
        private const string ProxyClassNameFormater = "{0}Proxy";
        private const MethodAttributes GetSetMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Virtual | MethodAttributes.HideBySig;

        public static T Create<T>(params object[] args) where T : ObTermBase
        {
            var proxyType = typeof(T);
            return (T)Create(proxyType, args);
        }

        public static object Create(Type proxyType, params object[] args)
        {
            var assemblyName = new AssemblyName(DynamicAssemblyName);
            var assemblyBuilderAccess = AssemblyBuilderAccess.Run;
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, assemblyBuilderAccess);
            //动态创建模块
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(DynamicModuleName);
            var proxyClassName = string.Format(ProxyClassNameFormater, proxyType.Name);
            //动态创建类代理
            var typeBuilderProxy = moduleBuilder.DefineType(proxyClassName, TypeAttributes.Public, proxyType);
            var constructorArgs = args.Select(o => o.GetType()).ToArray();
            var methodProxySet = proxyType.GetMethod("ProxySet", BindingFlags.Instance | BindingFlags.NonPublic);
            var constructorBuilder = typeBuilderProxy.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArgs);
            var ilgCtor = constructorBuilder.GetILGenerator();
            if (methodProxySet != null)
            {
                ilgCtor.Emit(OpCodes.Ldarg_0);
                for (var i = 0; i < args.Length; i++)
                {
                    ilgCtor.Emit(OpCodes.Ldarg_S, i + 1);
                }
                ilgCtor.Emit(OpCodes.Call, methodProxySet);
            }
            ilgCtor.Emit(OpCodes.Ret);//返回

            //获取被代理对象的所有属性,循环属性进行重写
            var properties = proxyType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var propertyName = propertyInfo.Name;
                var propertyType = propertyInfo.PropertyType;
                if (propertyType == typeof(ObProperty))
                {
                    //动态创建字段和属性
                    var propertyBuilder = typeBuilderProxy.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
                    //重写属性的Get Set方法
                    var methodGet = typeBuilderProxy.DefineMethod("get_" + propertyName, GetSetMethodAttributes, propertyType, Type.EmptyTypes);
                    var getProperty = proxyType.GetMethod("GetProperty", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                    //il of get method
                    var ilGetMethod = methodGet.GetILGenerator();
                    ilGetMethod.Emit(OpCodes.Ldarg_0);   //this
                    ilGetMethod.Emit(OpCodes.Ldstr, propertyName);
                    ilGetMethod.Emit(OpCodes.Call, getProperty);
                    ilGetMethod.Emit(OpCodes.Ret); //return numA

                    //设置属性的Get 
                    propertyBuilder.SetGetMethod(methodGet);
                }
                //else if(propertyType.Contains(typeof(ObTermBase)))
                else if (typeof(ObTermBase).IsAssignableFrom(propertyType))
                {
                    //动态创建字段和属性
                    var propertyBuilder = typeBuilderProxy.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
                    //重写属性的Get Set方法
                    var methodGet = typeBuilderProxy.DefineMethod("get_" + propertyName, GetSetMethodAttributes, propertyType, Type.EmptyTypes);
                    //il of get method
                    var ilGetMethod = methodGet.GetILGenerator();
                    ilGetMethod.Emit(OpCodes.Ldarg_0);   //this
                    ilGetMethod.Emit(OpCodes.Ldstr, propertyName);
                    ilGetMethod.Emit(OpCodes.Newobj, propertyType.GetConstructor(new[] { proxyType, typeof(string) }));
                    ilGetMethod.Emit(OpCodes.Call, typeof(ObTerm).GetMethod("CreateProxyByEmit"));
                    ilGetMethod.Emit(OpCodes.Ret);

                    //设置属性的Get 
                    propertyBuilder.SetGetMethod(methodGet);
                }
            }

            //使用动态类创建类型
            var proxyClassType = typeBuilderProxy.CreateTypeInfo();
            //创建类实例
            var instance = Activator.CreateInstance(proxyClassType, args);
            return instance;
        }

        public static TSource Of<TSource>(this TSource source) where TSource : ObTermBase
        {
            return Create<TSource>(source);
        }

        public static object CreateProxyByEmit(ObTermBase source)
        {
            return Create(source.GetType(), source);
        }
    }
}
