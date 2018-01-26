namespace NaiIrisDecisionTree.Model
{
    public class IrisRecord
    {
        [Classificator]
        public decimal SepalLength { get; set; }

        [Classificator]
        public decimal SepalWidth { get; set; }

        [Classificator]
        public decimal PetalLength { get; set; }

        [Classificator]
        public decimal PetalWidth { get; set; }
        public string Classification { get; set; }
    }
}
