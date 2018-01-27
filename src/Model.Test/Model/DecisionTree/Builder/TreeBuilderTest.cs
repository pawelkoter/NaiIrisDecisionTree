using System.Collections.Generic;
using FluentAssertions;
using NaiIrisDecisionTree.Model;
using NaiIrisDecisionTree.Model.DecisionTree.Builder;
using NaiIrisDecisionTree.Model.Helpers;
using NUnit.Framework;

namespace NaiIrisDecisionTree.Test.Model.DecisionTree.Builder
{
    [TestFixture]
    public class TreeBuilderTest
    {
        [Test]
        public void CalculateEntropyTest_TwoClasses_EvenDistribution()
        {
            //Given
            var builder = new TreeBuilder();
            var trainingSet = new List<IrisRecord>
            {
                new IrisRecord {Classification = "Iris-setosa"},
                new IrisRecord {Classification = "Iris-versicolor"},
            };
            var classification = typeof(IrisRecord).GetClassificationProperty();

            var expected = 1.0;
            var delta = 0.01;

            //When
            var entropy = builder.CalculateEntropy(trainingSet, classification);

            //Then
            entropy.Should().BeInRange(expected - delta, expected + delta);
        }

        [Test]
        public void CalculateEntropyTest_TwoClasses_UnevenDistribution()
        {
            //Given
            var builder = new TreeBuilder();
            var trainingSet = new List<IrisRecord>
            {
                new IrisRecord {Classification = "Iris-setosa"},
                new IrisRecord {Classification = "Iris-versicolor"},
                new IrisRecord {Classification = "Iris-versicolor"},
                new IrisRecord {Classification = "Iris-versicolor"},
                new IrisRecord {Classification = "Iris-versicolor"},
                new IrisRecord {Classification = "Iris-versicolor"},
                new IrisRecord {Classification = "Iris-versicolor"},
                new IrisRecord {Classification = "Iris-versicolor"},
                new IrisRecord {Classification = "Iris-versicolor"},
                new IrisRecord {Classification = "Iris-versicolor"},
            };
            var classification = typeof(IrisRecord).GetClassificationProperty();

            var expected = 0.47;
            var delta = 0.01;

            //When
            var entropy = builder.CalculateEntropy(trainingSet, classification);

            //Then
            entropy.Should().BeInRange(expected - delta, expected + delta);
        }

        [Test]
        public void BuildTreeTest()
        {
            //Given
            var builder = new TreeBuilder();
            var trainingSet = new List<IrisRecord>
            {
                new IrisRecord{SepalLength = 5.1m, SepalWidth = 3.5m, PetalLength = 1.4m, PetalWidth = 0.2m, Classification = "Iris-setosa"},
                new IrisRecord{SepalLength = 4.9m, SepalWidth = 3.0m, PetalLength = 1.4m, PetalWidth = 0.3m, Classification = "Iris-setosa"},
                new IrisRecord{SepalLength = 6.5m, SepalWidth = 3.0m, PetalLength = 5.2m, PetalWidth = 2.0m, Classification = "Iris-virginica"},
                new IrisRecord{SepalLength = 6.2m, SepalWidth = 3.4m, PetalLength = 5.4m, PetalWidth = 2.3m, Classification = "Iris-virginica"},
                new IrisRecord{SepalLength = 5.9m, SepalWidth = 3.0m, PetalLength = 5.1m, PetalWidth = 1.8m, Classification = "Iris-virginica"},
            };

            //When
            var tree = builder.Build(trainingSet);

            var classification = tree.Evaluate(new IrisRecord { SepalLength = 5.4m, SepalWidth = 3.4m, PetalLength = 1.7m, PetalWidth = 0.2m });

            //Then
            classification.Should().BeEquivalentTo("Iris-setosa");
        }
    }
}
