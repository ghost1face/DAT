using DAT.Serialization;
using System;
using System.Collections.Generic;

namespace DAT
{
    public class DataComparer
    {
        public bool AreEqual<T>(T item1, T item2)
        {
            throw new NotImplementedException();
        }

        public bool QuickDeepCompare(object item1, object item2)
        {
            var item1Bytes = ObjectSerializer.ToBytes(item1);
            var item2Bytes = ObjectSerializer.ToBytes(item2);

            return MD5Generator.Generate(item1Bytes) == MD5Generator.Generate(item2Bytes);
        }

        public bool CompareCollection<T>(IEnumerable<T> item1, IEnumerable<T> item2)
        {
            throw new NotImplementedException();
        }

        public bool CompareObjects<T>(T item1, T item2)
        {
            throw new NotImplementedException();
        }
    }
}
