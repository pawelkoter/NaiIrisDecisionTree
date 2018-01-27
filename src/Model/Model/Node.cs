using System.Linq;
using System.Reflection;
using NaiIrisDecisionTree.Model.Helpers;

namespace NaiIrisDecisionTree.Model
{
    public class Node<T> : INode<T> where T : class
    {
        public INode<T> LeftChild { get; set; }

        public INode<T> RightChild { get; set; }

        public PropertyInfo Classifier { get; set; }

        public string ClassifierName => Classifier.Name;

        public decimal Threshold { get; set; }

        public string Evaluate(T instance)
        {
            var property = instance.GetType().GetClassifierProperties().Single(x => x.Name == ClassifierName);

            dynamic prop = property.GetValue(instance);

            decimal value = prop;

            return value < Threshold 
                ? LeftChild.Evaluate(instance)
                : RightChild.Evaluate(instance);
        }
    }
}
