using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDStudio
{
    class TargetFactory
    {
        private static List<Type> FindAllDerivedTypes<T>()
        {
            return FindAllDerivedTypes<T>(System.Reflection.Assembly.GetAssembly(typeof(T)));
        }

        private static List<Type> FindAllDerivedTypes<T>(System.Reflection.Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly.GetTypes().Where(t => t != derivedType && derivedType.IsAssignableFrom(t)).ToList();

        }

        public static List<string> GetTargetNames()
        {
            List<string> names = new List<string>();
            var targets = FindAllDerivedTypes<Target>();
            
            foreach (var target in targets)
            {
                if(!target.IsAbstract)
                {
                    names.Add(target.FullName);
                }
            }

            return names;
        }

        public static Target Create(string targetName)
        {
            return (Target)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(targetName);
        }
    }
}
