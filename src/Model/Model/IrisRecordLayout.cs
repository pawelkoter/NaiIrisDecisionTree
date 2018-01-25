using System;
using System.Globalization;
using FlatFile.Core;
using FlatFile.Delimited.Implementation;

namespace NaiIrisDecisionTree.Model
{
    public sealed class IrisRecordLayout : DelimitedLayout<IrisRecord>
    {
        public IrisRecordLayout()
        {
            this.WithDelimiter(",")
                .WithMember(x => x.SepalLength, c => c.WithTypeConverter<DecimalConverter>())
                .WithMember(x => x.SepalWidth, c => c.WithTypeConverter<DecimalConverter>())
                .WithMember(x => x.PetalLength, c => c.WithTypeConverter<DecimalConverter>())
                .WithMember(x => x.PetalWidth, c => c.WithTypeConverter<DecimalConverter>());
        }

        private class DecimalConverter : ITypeConverter
        {
            public bool CanConvertFrom(Type type)
            {
                return type == typeof(string);
            }

            public bool CanConvertTo(Type type)
            {
                return type == typeof(decimal);
            }

            public string ConvertToString(object source)
            {
                return (source as decimal?)?.ToString();
            }

            public object ConvertFromString(string source)
            {
                return decimal.Parse(source, CultureInfo.InvariantCulture);
            }
        }
    }
}
