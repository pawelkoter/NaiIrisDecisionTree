using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NaiIrisDecisionTree.Model.Helpers
{
    public static class TypeExtensions
    {
        public static PropertyInfo GetClassificationProperty(this Type type)
        {
            return type.GetProperties()
                       .Single(x => x.GetCustomAttributes(false)
                                     .Any(a => a.GetType() == typeof(ClassificationAttribute)));
        }

        public static IList<PropertyInfo> GetClassifierProperties(this Type type)
        {
            return type.GetProperties()
                       .Where(x => x.GetCustomAttributes(false)
                                    .Any(a => a.GetType() == typeof(ClassifierAttribute)))
                       .ToList();
        }
    }
}
