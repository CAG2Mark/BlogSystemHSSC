using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BlogSystemHSSC
{
    /// <summary>
    /// Wrapper for serialization which also allows for backups.
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Serializes a file to a given path (object must be serializable).
        /// </summary>
        /// <param name="objToSerial">The object to serialize.</param>
        /// <param name="path">The path to serialize to.</param>
        /// <returns>Whether the save was successful.</returns>
        public static bool SaveFile(object objToSerial, string path)
        {
            try
            {
                // write the main file
                using (var output = File.Create(path))
                {
                    XmlSerializer formatter = new XmlSerializer(objToSerial.GetType());
                    formatter.Serialize(output, objToSerial);
                };

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Loads an XML serialized file from a given path, and saves a backup if the load was successful.
        /// </summary>
        /// <param name="path">The path of the serialized file.</param>
        /// <param name="type">The type of object being loaded.</param>
        /// <returns>The loaded object. On failure, this is null.</returns>
        public static object LoadFile(string path, Type type)
        {
            XmlSerializer serializer =
                new XmlSerializer(type);

            Object toReturn = null;

            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    // load the file and save to the Blog property
                    toReturn = serializer.Deserialize(fs);
                }

                // save backup file
                // this backup will always be valid since the backup is only saved when the blog has been read without errors.
                try
                {
                    File.Copy(path, $"{path}.bak");
                }
                catch (IOException)
                {
                }

                return toReturn;
            }
            catch (Exception)
            {
                try
                {
                    using (var fs = new FileStream($"{path}.bak", FileMode.Open))
                    {
                        // load the file and save to the Blog property
                        toReturn = serializer.Deserialize(fs);
                    }
                }
                catch (Exception)
                {
                    return null;
                }
                return toReturn;
            }
        }
    }
}
