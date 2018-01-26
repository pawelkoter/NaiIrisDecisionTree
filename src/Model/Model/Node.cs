using System.Linq;

namespace NaiIrisDecisionTree.Model
{
    public class Node<T> : INode<T> where T : class
    {
        public INode<T> LeftChild { get; set; }

        public INode<T> RightChild { get; set; }

        public string ClasificatorName { get; set; }

        public decimal Threshold { get; set; }

        public string Evaluate(T instance)
        {
            var property = instance.GetType().GetProperties().Where(x =>
                x.GetCustomAttributes(false).Any(a => a.GetType() == typeof(ClassificatorAttribute))).Single(x => x.Name == ClasificatorName);

            dynamic prop = property.GetValue(instance);

            decimal value = prop;

            return value < Threshold 
                ? LeftChild.Evaluate(instance)
                : RightChild.Evaluate(instance);
        }
    }
}
