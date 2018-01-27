using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NaiIrisDecisionTree.Model.Helpers;

namespace NaiIrisDecisionTree.Model.DecisionTree.Builder
{
    public class TreeBuilder
    {
        private const string DefaultClass = "Undefined";

        public DecisionTree<IrisRecord> Build(IList<IrisRecord> trainingSet)
        {
            var tree = new DecisionTree<IrisRecord>();

            var classifiers = typeof(IrisRecord).GetClassifierProperties();

            var classification = typeof(IrisRecord).GetClassificationProperty();

            tree.Root = ConstructTree(trainingSet, classifiers, classification, DefaultClass);

            return tree;
        }

        private INode<IrisRecord> ConstructTree(IList<IrisRecord> trainingSet, IList<PropertyInfo> classifiers, PropertyInfo classification, string defaultClass)
        {
            if (!trainingSet.Any())
            {
                return new Leaf<IrisRecord>{Value = defaultClass};
            }

            var classificationsCount = trainingSet.GroupBy(classification.GetValue)
                                                  .Count();
            if (classificationsCount == 1)
            {
                var classificationValue = (string) classification.GetValue(trainingSet.First());
                return new Leaf<IrisRecord> {Value = classificationValue};
            }

            var node = BestSplit(trainingSet, classifiers, classification, defaultClass);

            return node;
        }

        private INode<IrisRecord> BestSplit(IList<IrisRecord> trainingSet, IList<PropertyInfo> classifiers, PropertyInfo classification, string defaultClass)
        {
            var split = classifiers.Select(x => BestSplit(trainingSet, x, classification))
                                   .OrderByDescending(x => x.InformationGain)
                                   .First();

            var leftSet = trainingSet.Where(x => (decimal) split.Classifier.GetValue(x) < split.Threshold)
                                     .ToList();
            var rightSet = trainingSet.Except(leftSet)
                                       .ToList();

            var leftChild = ConstructTree(leftSet, classifiers, classification, defaultClass);
            var rightChild = ConstructTree(rightSet, classifiers, classification, defaultClass);

            var node = new Node<IrisRecord>
            {
                LeftChild = leftChild,
                RightChild = rightChild,
                Classifier = split.Classifier,
                Threshold = split.Threshold
            };

            return node;
        }

        private Split BestSplit(IList<IrisRecord> trainingSet, PropertyInfo classifier, PropertyInfo classification)
        {
            var orderedSet = trainingSet.OrderBy(x => (decimal) classifier.GetValue(x))
                                        .ToList();
            var currentEntropy = CalculateEntropy(trainingSet, classification);
            var splits = new List<Split>();

            for (var i = 0; i < orderedSet.Count - 1; i++)
            {
                var threshold = ((decimal) classifier.GetValue(orderedSet[i]) +
                                 (decimal) classifier.GetValue(orderedSet[i + 1])) / 2;

                var leftSet = trainingSet.Take(i + 1)
                                         .ToList();
                var rightSet = trainingSet.Skip(i + 1)
                                          .ToList();

                var splitEntropy = (double)leftSet.Count/trainingSet.Count*CalculateEntropy(leftSet, classification) 
                                   + (double)rightSet.Count/trainingSet.Count*CalculateEntropy(rightSet, classification);
                
                var split = new Split
                {
                    Threshold = threshold,
                    InformationGain = currentEntropy - splitEntropy,
                    Classifier = classifier
                };
                splits.Add(split);
            }

            var bestSplit = splits.OrderByDescending(x => x.InformationGain)
                                  .First();

            return bestSplit;
        }

        public double CalculateEntropy(IList<IrisRecord> trainingSet, PropertyInfo classification)
        {

            var classesFrequency = trainingSet.GroupBy(classification.GetValue)
                                        .Select(x => new Frequency
                                        {
                                            Class = x.Key,
                                            Count = x.Count()
                                        }).ToList();

            var entropy = classesFrequency.Sum(x => - (double)x.Count / trainingSet.Count * Math.Log((double)x.Count/trainingSet.Count, 2));

            return entropy;
        }

        private class Split
        {
            public double InformationGain { get; set; }
            public decimal Threshold { get; set; }
            public PropertyInfo Classifier { get; set; }
        }

        private class Frequency
        {
            public object Class { get; set; }
            public int Count { get; set; }
        }
    }
}