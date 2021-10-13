using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace UtilitiesLib
{
    public class ListManager<T> : IListManager<T>
    {
        private List<T> list;
        private int count;

        public ListManager()
        {
            list = new List<T>();
        }

        // Properties
        public int Count
        {
            get { return count; }
        }

        public List<T> List
        {
            get { return list; }
        }

        /// <summary>
        /// Add a new object to the list.
        /// </summary>
        /// <param name="aType"></param>
        /// <returns></returns>
        public bool Add(T aType)
        {
            bool success;

            if (aType != null)
            {
                list.Add(aType);
                count++;
                success = true;
            }
            else
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Replace (change) an object in a position with a new object. 
        /// </summary>
        /// <param name="aType"></param>
        /// <param name="anIndex"></param>
        /// <returns></returns>
        public bool ChangeAt(T aType, int anIndex)
        {
            bool success = false;

            if (aType != null && anIndex > -1)
            {
                list[anIndex] = aType;
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Control that a given index is >= 0 and less than the number of items in 
        /// the collection.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool CheckIndex(int index)
        {
            bool indexOk = false;

            if (index >= 0 && index < Count)
            {
                indexOk = true;
            }

            return indexOk;
        }

        /// <summary>
        /// Deletes all object of the collection and set the collection to null.
        /// </summary>
        public void DeleteAll()
        {
            list.Clear();
            count = 0;
        }

        /// <summary>
        /// Remove an object from the list at a certain position.
        /// </summary>
        /// <param name="anIndex"></param>
        /// <returns></returns>
        public bool DeleteAt(int anIndex)
        {
            bool success = CheckIndex(anIndex);

            if (success)
            {
                list.RemoveAt(anIndex);
                count--;
                success = true;
            }
            else
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Return an object from a certain position in the list.
        /// </summary>
        /// <param name="anIndex"></param>
        /// <returns></returns>
        public T GetAt(int anIndex)
        {
            T obj;
            bool indexOk = CheckIndex(anIndex);

            if (indexOk)
            {
                obj = list[anIndex];
            }
            else
            {
                obj = default(T);
            }
            return obj;
        }

        /// <summary>
        /// Return an array of strings where every string is represents the object 
        /// (calling the ToString() of the object). 
        /// </summary>
        /// <returns></returns>
        public string[] ToStringArray()
        {
            string[] stringArray = new string[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                string strAti = list[i].ToString();
                stringArray[i] = strAti;
            }

            return stringArray;
        }

        /// <summary>
        /// Return a of strings where every string is represents the object 
        /// (calling the ToString() of the object). 
        /// </summary>
        /// <returns></returns>
        public List<string> ToStringList()
        {
            List<string> stringList = null;

            for (int i = 0; i < list.Count; i++)
            {
                stringList[i] = list.ElementAt(i).ToString();
            }

            return stringList;
        }

        /// <summary>
        /// Deserialize an object from a binary file.
        /// Not safe to use.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool BinaryDeSerialize(string fileName)
        {
            bool success = false;

            if (fileName != null)
            {
                using (Stream stream = File.Open(fileName, FileMode.Open))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    DeleteAll();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                    list = (List<T>)binaryFormatter.Deserialize(stream);
                    count = list.Count;
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Serialize an object to binary file.
        /// Not safe to use.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool BinarySerialize(string fileName)
        {
            bool success = false;

            if (fileName != null)
            {
                using (Stream stream = File.Open(fileName, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                    binaryFormatter.Serialize(stream, list);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Writes the list to an XML file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool XMLSerialize(string fileName)
        {
            bool success = false;

            if (fileName != null)
            {
                TextWriter writer = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                    writer = new StreamWriter(fileName);
                    serializer.Serialize(writer, list);
                    success = true;
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }
            return success;
        }

        public bool XMLDeserialize(string fileName)
        {
            bool success = false;
            TextReader reader = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                reader = new StreamReader(fileName);
                list = (List<T>)serializer.Deserialize(reader);
                success = true;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return success;
        }
    }
}
