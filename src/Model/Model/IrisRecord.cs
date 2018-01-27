namespace NaiIrisDecisionTree.Model
{
    public class IrisRecord
    {
        [Classifier]
        public decimal SepalLength { get; set; }

        [Classifier]
        public decimal SepalWidth { get; set; }

        [Classifier]
        public decimal PetalLength { get; set; }

        [Classifier]
        public decimal PetalWidth { get; set; }

        [Classification]
        public string Classification { get; set; }
    }
}
