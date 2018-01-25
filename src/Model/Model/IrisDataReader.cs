using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlatFile.Delimited.Implementation;

namespace NaiIrisDecisionTree.Model
{
    public class IrisDataReader
    {
        public IList<IrisRecord> Read(Stream stream)
        {
            var layout = new IrisRecordLayout();
            var factory = new DelimitedFileEngineFactory();
            var flatFile = factory.GetEngine(layout);

            var records = flatFile.Read<IrisRecord>(stream).ToList();

            return records;
        }
    }
}
