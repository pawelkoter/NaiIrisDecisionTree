using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using NaiIrisDecisionTree.Model;
using NUnit.Framework;

namespace NaiIrisDecisionTree.Test.Model
{
    [TestFixture]
    public class IrisDataReaderTest
    {
        [Test]
        public void Test()
        {
            //Given

            var file = new StringBuilder().AppendLine("5.1,3.5,1.4,0.2,Iris-setosa")
                                          .AppendLine("4.7,3.2,1.3,0.2,Iris-setosa")
                                          .AppendLine("5.9,3.2,4.8,1.8,Iris-versicolor")
                                          .AppendLine("6.0,2.2,5.0,1.5,Iris-virginica")
                                          .ToString();

            var fileBuffer = Encoding.ASCII.GetBytes(file);

            var reader = new IrisDataReader();
            IList<IrisRecord> records;
            
            //When
            using (var stream = new MemoryStream(fileBuffer))
            {
                records = reader.Read(stream);
            }

            //Then
            records.Should().HaveCount(4, "Because it is number of records in the file");
        }
    }
}
