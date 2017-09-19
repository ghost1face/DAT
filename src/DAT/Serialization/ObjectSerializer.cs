using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DAT.Serialization
{
    public static class ObjectSerializer
    {
        public static byte[] ToBytes(object serializeMe)
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(memoryStream, serializeMe);

                return memoryStream.ToArray();
            }
        }
    }
}
