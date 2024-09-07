using System;
using System.Collections.Generic;
using System.Linq;


// TODO: Move this to another package.
namespace Match3.Development.Base.DevelopmentConsole
{
    public static class ReflectionUtilities
    {
        public static List<Type> GetTypesOf(Type type, bool shouldConsiderAbstracts)
        {
            var allTypes = GetAllTypes(type);
            List<Type> targetTypes = new List<Type>();

            foreach (var t in allTypes)
            {
                if (type.IsAssignableFrom(t))
                {
                    if (shouldConsiderAbstracts)
                        targetTypes.Add(t);
                    else if (!t.IsInterface && !t.IsAbstract)
                        targetTypes.Add(t);
                }
            }

            return targetTypes;
        }

        public static List<string> FindTypesOf(Type type, bool shouldConsiderAbstracts)
        {
            List<Type> targetTypes = GetTypesOf(type, shouldConsiderAbstracts);
            return new List<string>(targetTypes.Select(t => t.ToString()));
        }

        public static Type[] GetAllTypes(Type aType)
        {
            var result = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                result.AddRange(assembly.GetTypes());

            return result.ToArray();
        }

        // NOTE: Why not Type.GetType(typeName)?!!
        public static Type GetType(string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                if (assembly.GetType(typeName) != null)
                    return assembly.GetType(typeName);

            return null;
        }
    }
}