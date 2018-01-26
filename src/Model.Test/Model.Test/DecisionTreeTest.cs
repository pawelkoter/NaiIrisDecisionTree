﻿using FluentAssertions;
using NUnit.Framework;

namespace NaiIrisDecisionTree.Model.Test
{
    [TestFixture]
    public class DecisionTreeTest
    {
        [Test]
        public void Test()
        {
            //Given
            var decisionTree = new DecisionTree<IrisRecord>
            {
                Root = new Node<IrisRecord>
                {
                    LeftChild = new Leaf<IrisRecord> {Value = "Iris-setosa"},
                    RightChild = null,
                    Threshold = 4.0m,
                    ClasificatorName = nameof(IrisRecord.SepalWidth)
                }
            };

            //When
            var result = decisionTree.Evaluate(new IrisRecord
            {
                SepalLength = 5.1m,
                SepalWidth = 3.5m,
                PetalLength = 1.4m,
                PetalWidth = 0.2m,
            });

            //Then
            result.Should().Be("Iris-setosa");
        }
        
    }
}