namespace NaiIrisDecisionTree.Model
{
    public interface INode<T> where T : class
    {
        string Evaluate(T instance);
    }
}
