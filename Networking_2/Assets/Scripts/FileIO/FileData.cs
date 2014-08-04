using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
#if ONLOOKER_UNITY_ENGINE
using UnityEngine;
#endif
namespace OnLooker
{
        /// <summary>
        /// This class holds SaveData and can save and load it.
        /// </summary>
        public class FileData
        {
            private List<SaveData> m_Data;
            private FileHeader m_FileHeader;

            /// <summary>
            /// Creates the data as well as the file header
            /// </summary>
            /// <param name="aName"></param>
            public FileData(string aName)
            {
                aName = getPath(aName);
                m_Data = new List<SaveData>();
                m_FileHeader = new FileHeader(aName, 1);
                m_Data.Add(m_FileHeader);
            }


            /// <summary>
            /// Get data by name
            /// </summary>
            /// <param name="aName"></param>
            /// <returns></returns>
            public SaveData get(string aName)
            {
                if (m_Data != null)
                {
                    for (int i = 0; i < m_Data.Count; i++)
                    {
                        if (m_Data[i] != null)
                        {
                            if (m_Data[i].name == aName)
                            {
                                return m_Data[i];
                            }
                        }
                    }
                }
                return null;
            }
            /// <summary>
            /// Get data by name for type
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="aName"></param>
            /// <returns></returns>
            /// 
            

            public T get<T>(string aName) where T: class
            {
                for (int i = 0; i < m_Data.Count; i++)
                {
                    if (m_Data[i].name == aName)
                    {
                        return (m_Data[i] as T);
                    }
                }

                return null;
            }

            /// <summary>
            /// Get data by type as an array
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T[] get<T>() where T : class
            {
                List<T> types = new List<T>();
                for (int i = 0; i < m_Data.Count; i++)
                {
                    if (m_Data[i].GetType() == typeof(T))
                    {
                        types.Add(m_Data[i] as T);
                    }
                }
                return types.ToArray();
            }

            /// <summary>
            /// Add data into the file data
            /// </summary>
            /// <param name="aData"></param>
            public void add(SaveData aData)
            {
                if (aData != null)
                {
                    m_Data.Add(aData);
                    m_FileHeader.itemCount = m_Data.Count;
                }
            }
            /// <summary>
            /// Add data into the file data and sets the data name
            /// </summary>
            /// <param name="aData"></param>
            /// <param name="aDataName"></param>
            public void add(SaveData aData, string aDataName)
            {
                if (aData != null)
                {
                    aData.name = aDataName;
                    m_Data.Add(aData); 
                    m_FileHeader.itemCount = m_Data.Count;
                }
            }
            /// <summary>
            /// Removes the data by reference
            /// </summary>
            /// <param name="aData"></param>
            public void remove(SaveData aData)
            {
                if (m_Data != null)
                {
                    for (int i = 0; i < m_Data.Count; i++)
                    {
                        if (m_Data[i] == aData)
                        {
                            m_Data.RemoveAt(i);
                            return;
                        }
                    }
                }
            }
            /// <summary>
            /// Removes the data by name
            /// </summary>
            /// <param name="aDataName"></param>
            public void remove(string aDataName)
            {
                if (m_Data != null)
                {
                    for (int i = 0; i < m_Data.Count; i++)
                    {
                        if (m_Data[i] != null)
                        {
                            if (m_Data[i].name == aDataName)
                            {
                                m_Data.RemoveAt(i);
                                return;
                            }
                        }
                    }
                }
            }


            /// <summary>
            /// Saves the data to the hard disc using the file header filename
            /// </summary>
            public void save()
            {

                BinaryFormatter formatter = new BinaryFormatter();
                Stream stream = File.Open(m_FileHeader.filename, FileMode.Create, FileAccess.Write);
                for (int i = 0; i < m_FileHeader.itemCount; i++)
                {
                    m_Data[i].save(stream, formatter);
                }

                stream.Close();
            }
            /// <summary>
            /// Loads the data from the hard disc using the file header filename
            /// </summary>
            public void load()
            {
                //Check the file
                if (!File.Exists(m_FileHeader.filename))
                {
                    //ERROR BAD FILE
                    return;
                }
                //Check the file header
                BinaryFormatter formatter = new BinaryFormatter();
                Stream stream = File.Open(m_FileHeader.filename, FileMode.Open, FileAccess.Read);
                try
                {
                    m_FileHeader = (FileHeader)formatter.Deserialize(stream);
                }
                catch (Exception e)
                {
                    //ERROR BAD FILE HEADER
                    return;
                }
                //Retrieve all the data from the file
                if (m_FileHeader != null)
                {
                    m_Data.Clear();
                    m_Data.Add(m_FileHeader);
                    for (int i = 1; i < m_FileHeader.itemCount; i++)
                    {
                        m_Data.Add((SaveData)formatter.Deserialize(stream));
                    }
                }
                stream.Close();
            }


            /// <summary>
            /// Deletes the previous file and saves a new one with the new name
            /// </summary>
            /// <param name="aName"></param>
            public void rename(string aName)
            {
                aName = getPath(aName);
                File.Delete(m_FileHeader.filename);
                m_FileHeader.filename = aName;
                save();
            }
            /// <summary>
            /// Clears the data from the FileData instance. Does not clear the data on the hard disc.
            /// </summary>
            public void clear()
            {
                m_Data.Clear();
                m_Data.Add(m_FileHeader);
                m_FileHeader.itemCount = m_Data.Count;
            }
            /// <summary>
            /// Clears the data from the FileData instance and deletes the data on the hard disc.
            /// </summary>
            public void delete()
            {
                m_Data.Clear();
                m_Data.Add(m_FileHeader);
                m_FileHeader.itemCount = m_Data.Count;
                File.Delete(m_FileHeader.filename);
            }

            private string getPath(string aFile)
            {
#if ONLOOKER_UNITY_ENGINE
                return Application.dataPath + "/" + aFile;
#else
                return aFile;
#endif
            }
        }
}
