using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;

namespace HUF.Utils.Runtime.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serializes an object to byte array.
        /// </summary>
        /// <param name="obj">A serializable object.</param>
        /// <returns>Byte array.</returns>
        [PublicAPI]
        public static byte[] SerializeToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes byte array to an object of a type.
        /// </summary>
        /// <param name="byteArray">Byte array with binary data.</param>
        /// <typeparam name="T">A type of object.</typeparam>
        /// <returns>An object of a given type.</returns>
        [PublicAPI]
        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}