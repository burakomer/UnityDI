// using System;
// using System.Collections.Generic;
//
// namespace DwarfEngine.DependencyKit.Experimental
// {
//     public static class BinderTest
//     {
//         internal static Dictionary<object, Binding> bindings;
//
//         static BinderTest()
//         {
//             bindings = new Dictionary<object, Binding>();
//         }
//         
//         public static IBindingSource Bind<T>(this object obj)
//         {
//             var source = new BindingSource<T>(obj);
//             return source;
//         }
//         
//         public interface IBindingSource
//         {
//             void To<TInstance>() where TInstance : class, new();
//         }
//     
//         public class BindingSource<TSource> : IBindingSource
//         {
//             private object obj;
//             
//             public BindingSource(object obj)
//             {
//                 this.obj = obj;
//             }
//
//             public void To<TInstance>() where TInstance : class, new()
//             {
//                 var binding = new Binding(typeof(TSource), typeof(TInstance));
//                 bindings.Add(obj, binding);
//             }
//         }
//
//         public class Binding
//         {
//             internal readonly Type sourceType;
//             internal readonly Type instanceType;
//         
//             public Binding(Type sourceType, Type instanceType)
//             {
//                 this.sourceType = sourceType;
//                 this.instanceType = instanceType;
//             }
//         }
//     }
//     
//     
// }