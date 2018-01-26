namespace NaiIrisDecisionTree.Model
{
    public class DecisionTree<T> where T : class
    {
        public Node<T> Root { get; set; }

        public string Evaluate(T instance)
        {
            return Root.Evaluate(instance);
        }
    }
}