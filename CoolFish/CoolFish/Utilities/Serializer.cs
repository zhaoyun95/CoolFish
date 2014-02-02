using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     Helper Serializer class to help with saving things to files
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        ///     Deserialize a previously stored object
        /// </summary>
        /// <typeparam name="T">Type of object stored in the file</typeparam>
        /// <param name="filename">name of the file that the object is stored in</param>
        /// <returns>object of type T retrieved from the file; null if the object couldn't be read</returns>
        public static T DeSerialize<T>(string filename)
        {
            var fs = new FileStream(filename, FileMode.Open);
            var formatter = new BinaryFormatter();
            try
            {
                return (T) formatter.Deserialize(fs);
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Serializes an generic item
        /// </summary>
        /// <param name="path">File path to save to</param>
        /// <param name="objectToSerialize">generic object to serialize</param>
        /// <typeparam name="T">The generic type to serialize</typeparam>
        public static void Serialize<T>(string path, T objectToSerialize)
        {
            var fs = new FileStream(path, FileMode.Create);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            var formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, objectToSerialize);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}