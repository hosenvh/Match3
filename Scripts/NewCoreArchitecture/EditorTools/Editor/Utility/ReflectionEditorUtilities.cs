using System;
using System.Collections.Generic;


namespace Match3.EditorTools.Editor.Utility
{
    public static class ReflectionEditorUtilities
    {
        public static List<Type> FindTypesOf(Type type, bool considerAbstracts)
        {
            //var allTypes = new List<Type>();
            //foreach(var assembly in CompilationPipeline.GetAssemblies())
            //{
            //    allTypes.AddRange(assembly..GetTypes())
            //}

            //var assembely = Assembly.GetExecutingAssembly();
            var allTypes = GetAllTypes(type);
            List<Type> targetTypes = new List<Type>();

            foreach (var t in allTypes)
            {
                if (type.IsAssignableFrom(t))
                {
                    if (considerAbstracts)
                        targetTypes.Add(t);
                    else if (!t.IsInterface && !t.IsAbstract)
                        targetTypes.Add(t);
                }
            }

            return targetTypes;
        }

        public static Type[] GetAllTypes(Type aType)
        {
            var result = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                result.AddRange(assembly.GetTypes());

            return result.ToArray();
        }


        // TODO: Refactor this.
        public static Type GetType(string typeName)
        {
            if (typeName.IsNullOrEmpty())
                return null;

            var type = Type.GetType(typeName);
            if (type != null)
                return type;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                if (assembly.GetType(typeName) != null)
                    return assembly.GetType(typeName);

            return null;
        }

    }
}