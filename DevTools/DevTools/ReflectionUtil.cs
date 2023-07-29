using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullModdedFuriesAPI;

namespace DevMenu
{
    public class ReflectionUtil
    {
         /// <summary>Get a method from the type hierarchy.</summary>
        /// <param name="type">The type which has the method.</param>
        /// <param name="obj">The object which has the method.</param>
        /// <param name="name">The method name.</param>
        /// <param name="bindingFlags">The reflection binding which flags which indicates what type of method to find.</param>
        /// <summary>Get a method from the type hierarchy.</summary>
        /// <param name="type">The type which has the method.</param>
        /// <param name="obj">The object which has the method.</param>
        /// <param name="name">The method name.</param>
        /// <param name="bindingFlags">The reflection binding which flags which indicates what type of method to find.</param>
        /// <param name="argumentTypes">The argument types of the method signature to find.</param>
        // private ReflectedMethod GetMethodFromHierarchy(Type type, object obj, string name, BindingFlags bindingFlags, Type[] argumentTypes)
        // {
        //     bool isStatic = bindingFlags.HasFlag(BindingFlags.Static);
        //     MethodInfo method = this.GetCached($"method::{isStatic}::{type.FullName}::{name}({string.Join(",", argumentTypes.Select(p => p.FullName))})", () =>
        //     {
        //         MethodInfo methodInfo = null;
        //         for (; type != null && methodInfo == null; type = type.BaseType)
        //             methodInfo = type.GetMethod(name, bindingFlags, null, argumentTypes, null);
        //         return methodInfo;
        //     });
        //     return method != null
        //         ? new ReflectedMethod(type, obj, method, isStatic)
        //         : null;
        // }
        //
        // public IReflectedMethod GetMethod(object obj, string name, Type[] argumentTypes, bool required = true)
        // {
        //     // validate parent
        //     if (obj == null)
        //         throw new ArgumentNullException(nameof(obj), "Can't get a instance method from a null object.");
        //
        //     // get method from hierarchy
        //     ReflectedMethod method = this.GetMethodFromHierarchy(obj.GetType(), obj, name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, argumentTypes);
        //     if (required && method == null)
        //         throw new InvalidOperationException($"The {obj.GetType().FullName} object doesn't have a '{name}' instance method with that signature.");
        //     return method;
        // }


         public IEnumerable<Type> FindDerivedTypesFromAssembly(Assembly assembly, Type baseType, bool classOnly)
         {
             if (assembly == null)
                 throw new ArgumentNullException("assembly", "Assembly must be defined");
             if (baseType == null)
                 throw new ArgumentNullException("baseType", "Parent Type must be defined");

             // get all the types
             var types = assembly.GetTypes();

             // works out the derived types
             foreach (var type in types)
             {
                 // if classOnly, it must be a class
                 // useful when you want to create instance
                 if (classOnly && !type.IsClass)
                     continue;

                 if (baseType.IsInterface)
                 {
                     var it = type.GetInterface(baseType.FullName);

                     if (it != null)
                         // add it to result list
                         yield return type;
                 }
                 else if (type.IsSubclassOf(baseType))
                 {
                     // add it to result list
                     yield return type;
                 }
             }
         }

         public static IEnumerable<T> GetAllImplementingInstancesOfInterface<T>()
         {
             var objects = new List<T>();
             foreach (var assembly in GetAvailableAssemblies())
             {
                 foreach (var type in assembly.GetTypes()
                     .Where(
                         myType => myType.IsClass &&
                                   !myType.IsAbstract &&
                                   !myType.IsGenericType &&
                                   myType.GetInterfaces().Contains(typeof(T))))
                 {
                     objects.Add((T)Activator.CreateInstance(type));
                 }
             }

             return objects;
         }

         public static IEnumerable<Assembly> GetAvailableAssemblies()
         {
             return AppDomain.CurrentDomain.GetAssemblies();
         }
    }
}
