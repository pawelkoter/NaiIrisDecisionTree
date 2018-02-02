using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using NaiIrisDecisionTree.Model;
using NaiIrisDecisionTree.Model.DecisionTree.Builder;

namespace NaiIrisDecisionTree.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IrisDataReader _irisDataReader;
        private readonly TreeBuilder _builder;
        private DecisionTree<IrisRecord> _decisionTree;
        private IList<IrisRecord> _irisRecords;
        private IList<IrisRecord> _trainingSet;
        private IList<IrisRecord> _testSet;

        public MainWindow()
        {
            InitializeComponent();
            _irisDataReader = new IrisDataReader();
            _builder = new TreeBuilder();
        }

        private void ChooseFile(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == true)
                {
                    FilePath.Text = openFileDialog.FileName;
                    LoadData.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadFile(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var stream = new FileStream(FilePath.Text, FileMode.Open))
                {
                    _irisRecords = _irisDataReader.Read(stream);
                    IrisRecords.ItemsSource = _irisRecords;
                    GenerateTrainingSetButton.IsEnabled = true;
                    TrainingSetSize.Maximum = _irisRecords.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateTrainingSet(object sender, RoutedEventArgs e)
        {
            try
            {
                var count = (int)TrainingSetSize.Value;
                _trainingSet = _irisRecords.OrderBy(elem => Guid.NewGuid())
                                           .Take(count)
                                           .ToList();
                _testSet = _irisRecords.Except(_trainingSet)
                                       .ToList();
                TrainingSetDataGrid.ItemsSource = _trainingSet;
                GenerateTreeButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateTree(object sender, RoutedEventArgs e)
        {
            try
            {
                _decisionTree = _builder.Build(_trainingSet);
                RunTestDataButton.IsEnabled = true;
                BuildTreeView(_decisionTree);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuildTreeView(DecisionTree<IrisRecord> decisionTree)
        {
            Tree.Items.Add(BuildTreeView(decisionTree.Root, "Root"));
        }

        private TreeViewItem BuildTreeView(INode<IrisRecord> iNode, string name)
        {
            if (iNode is Node<IrisRecord> node)
            {
                return BuildTreeView(node, name);
            }

            if (iNode is Leaf<IrisRecord> leaf)
            {
                return BuildTreeView(leaf, name);
            }

            return null;

        }

        private TreeViewItem BuildTreeView(Node<IrisRecord> node, string name)
        {
            var item = new TreeViewItem
            {
                Header = name,
                IsExpanded = true
            };

            item.Items.Add($"Classifier: {node.ClassifierName}");
            item.Items.Add($"Threshold: {node.Threshold}");
            item.Items.Add(BuildTreeView(node.LeftChild, "Left"));
            item.Items.Add(BuildTreeView(node.RightChild, "Right"));

            return item;
        }

        private TreeViewItem BuildTreeView(Leaf<IrisRecord> leaf, string name)
        {
            var item = new TreeViewItem
            {
                Header = name,
                IsExpanded = true

            };
            item.Items.Add($"Classification: {leaf.Value}");
            return item;
        }

        private void RunTestData(object sender, RoutedEventArgs e)
        {
            try
            {
                var testResults = _testSet.Select(x => new IrisTestRecord
                {
                    PetalLength = x.PetalLength,
                    PetalWidth = x.PetalWidth,
                    SepalLength = x.SepalLength,
                    SepalWidth = x.SepalWidth,
                    Classification = x.Classification,
                    EvaluatedClassification = _decisionTree.Evaluate(x)
                }).ToList();

                var groups = testResults.GroupBy(x => x.Classification).ToList();
                var statistics = new List<EvaluationStatistic>();
                foreach (var group in groups)
                {
                    var matched = group.Where(x => x.Classification == x.EvaluatedClassification)
                                       .GroupBy(x => x.EvaluatedClassification)
                                       .Select(x => new EvaluationStatistic
                                       {
                                           Classification = group.Key,
                                           EvaluatedClassification = x.Key,
                                           Percentage = Math.Round((decimal)x.Count() / group.Count() * 100, 1)
                                       }).ToList();

                    var unmatched = group.Where(x => x.Classification != x.EvaluatedClassification)
                                         .GroupBy(x => x.EvaluatedClassification)
                                         .Select(x => new EvaluationStatistic
                                         {
                                             Classification = group.Key,
                                             EvaluatedClassification = x.Key,
                                             Percentage = Math.Round((decimal)x.Count()/group.Count() * 100, 1)
                                         }).ToList();

                    statistics.AddRange(matched);
                    statistics.AddRange(unmatched);
                }

                TestResult.ItemsSource = testResults;
                Statistics.ItemsSource = statistics;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            _decisionTree = null;
            _irisRecords = null;
            _trainingSet = null;
            _testSet = null;

            IrisRecords.ItemsSource = null;
            TrainingSetDataGrid.ItemsSource = null;
            TestResult.ItemsSource = null;
            Statistics.ItemsSource = null;

            RunTestDataButton.IsEnabled = false;
            GenerateTreeButton.IsEnabled = false;
            GenerateTrainingSetButton.IsEnabled = false;

            Tree.Items.Clear();
        }
    }
}
