using System.IO;
using System.Xml.Serialization;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     Helper Serializer class to help with saving things to files
    /// </summary>
    public class Serializer
    {
        /// <summary>
        ///     Serializes and object and saves it to a file
        /// </summary>
        /// <param name="filename">name of the file to save to</param>
        /// <param name="objectToSerialize">object to save to a file</param>
        public static void SerializeObject(string filename, object objectToSerialize)
        {
            var serializer = new XmlSerializer(objectToSerialize.GetType());
            Stream stream = File.Open(filename, FileMode.Create);
            serializer.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        /// <summary>
        ///     Deserialize a previously stored object
        /// </summary>
        /// <typeparam name="T">Type of object stored in the file</typeparam>
        /// <param name="filename">name of the file that the object is stored in</param>
        /// <returns>object of type T retrieved from the file; null if the object couldn't be read</returns>
        public static T DeSerializeObject<T>(string filename)
        {
            var serializer = new XmlSerializer(typeof (T));

            Stream stream = File.Open(filename, FileMode.Open);
            object theObject = (T) serializer.Deserialize(stream);
            stream.Close();
            return (T) theObject;
        }
    }
}