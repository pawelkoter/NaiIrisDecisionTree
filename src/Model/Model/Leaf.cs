namespace NaiIrisDecisionTree.Model
{
    public class Leaf<T> : INode<T> where T : class 
    {
        public string Value { get; set; }

        public string Evaluate(T instance)
        {
            return Value;
        }
    }
}
