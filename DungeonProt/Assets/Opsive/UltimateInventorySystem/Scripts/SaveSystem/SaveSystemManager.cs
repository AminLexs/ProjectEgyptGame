/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.SaveSystem
{
    using Opsive.Shared.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// The save system manager is used by saver components to load and save any serializable data.
    /// </summary>
    public class SaveSystemManager : MonoBehaviour
    {
        [Tooltip("Load the last save data automatically on start.")]
        [SerializeField] protected bool m_AutoLoadLastSave;
        [Tooltip("Automatically save the game before the application quits.")]
        [SerializeField] protected bool m_AutoSaveOnApplicationQuit;
        [Tooltip("The save file name when saved on disk.")]
        [SerializeField] protected string m_SaveFileName = "SaveFile";
        [Tooltip("The maximum number of save files possible.")]
        [SerializeField] protected int m_MaxSaves = 100;

        protected SaveData m_SaveData;
        protected Dictionary<int, SaveData> m_Saves;
        protected List<SaverBase> m_Savers;

        protected SaveData[] m_SavesArray;
        #region Singleton and Static functions

#if UNITY_2019_3_OR_NEWER
        /// <summary>
        /// Reset the static variables for domain reloading.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void DomainReset()
        {
            s_Initialized = false;
            s_Instance = null;
        }
#endif

        private static SaveSystemManager s_Instance;
        public static SaveSystemManager Instance {
            get {
                if (!s_Initialized) {
                    s_Instance = new GameObject("SaveSystemManager").AddComponent<SaveSystemManager>();
                    s_Initialized = true;
                }
                return s_Instance;
            }
        }
        private static bool s_Initialized;

        /// <summary>
        /// Returns true if the InventorySystemManger was not Initialized.
        /// </summary>
        public static bool IsNull => s_Instance == null || s_Initialized == false;

        public static int MaxSaves => Instance.m_MaxSaves;

        public static IReadOnlyList<SaverBase> Savers => Instance.m_Savers;

        public static IReadOnlyDictionary<int, SaveData> Saves => Instance.m_Saves;

        /// <summary>
        /// The object has been enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            // The object may have been enabled outside of the scene unloading.
            if (!s_Initialized) {
                s_Instance = this;
                s_Initialized = true;
                SceneManager.sceneUnloaded -= SceneUnloaded;
            }
        }

        #region Register Unregister Savers

        public static void RegisterSaver(SaverBase saver)
        {
            Instance.RegisterSaverInternal(saver);
        }

        public static void UnregisterSaver(SaverBase saver)
        {
            Instance.UnregisterSaverInternal(saver);
        }

        #endregion

        #region Save & Load

        public static void AddToSaveData(string fullKey, Serialization serializedSaveData)
        {
            Instance.AddToSaveDataInternal(fullKey, serializedSaveData);
        }

        public static void Save(int saveIndex)
        {
            Instance.SaveInternal(saveIndex);
        }

        public static void Load(int saveIndex)
        {
            Instance.LoadInternal(saveIndex);
        }

        #endregion

        #region Getters & Setters

        public static bool TryGetSaveData(string fullKey, out Serialization serializedData)
        {
            return Instance.TryGetSaverDataInternal(fullKey, out serializedData);
        }

        public static IReadOnlyList<SaveData> GetSaves()
        {
            return Instance.GetSavesInternal();
        }

        public static SaveData GetCurrentSaveData()
        {
            return Instance.GetCurrentSaveDataInternal();
        }

        public static void SetCurrentSaveData(SaveData newSaveData)
        {
            Instance.SetCurrentSaveDataInternal(newSaveData);
        }

        #endregion

        #region Delete

        public static void DeleteSave(int saveIndex)
        {
            Instance.DeleteSaveInternal(saveIndex);
        }

        #endregion

        #region Singleton

        /// <summary>
        /// Destroys the object instance on the network.
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        public static void Destroy(GameObject obj)
        {
            if (s_Instance == null) {
                Debug.LogError("Error: Unable to destroy object - the Inventory System Manager doesn't exist.");
                return;
            }
            s_Instance.DestroyInternal(obj);
        }

        /// <summary>
        /// Reset the initialized variable when the scene is no longer loaded.
        /// </summary>
        /// <param name="scene">The scene that was unloaded.</param>
        protected virtual void SceneUnloaded(Scene scene)
        {
            s_Instance = null;
            s_Initialized = false;
            SceneManager.sceneUnloaded -= SceneUnloaded;
        }

        /// <summary>
        /// The object has been disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            SceneManager.sceneUnloaded += SceneUnloaded;
        }

        /// <summary>
        /// Do something when object is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            DestroyInternal(gameObject);
        }

        #endregion
        #endregion
        /// <summary>
        /// Called on Awake Initializes the Manager.
        /// </summary>
        protected virtual void Awake()
        {
            OnEnable();
            Initialize();
        }

        /// <summary>
        /// Initilaizes the Manager using the database if one is specified.
        /// </summary>
        public virtual void Initialize()
        {
            m_SavesArray = new SaveData[m_MaxSaves];
            m_Savers = new List<SaverBase>();
            m_Saves = new Dictionary<int, SaveData>();
            GetSavesFromDisk(ref m_Saves);

            for (int i = 0; i < m_SavesArray.Length; i++) {
                if (m_Saves.ContainsKey(i)) {
                    m_SavesArray[i] = m_Saves[i];
                }
            }

            if (m_AutoLoadLastSave && m_Saves != null && m_Saves.ContainsKey(0)) {
                m_SaveData = new SaveData(m_Saves[0]);
            } else {
                m_SaveData = new SaveData();
            }
        }

        /// <summary>
        /// Register the saver components
        /// </summary>
        /// <param name="saver">The saver.</param>
        protected virtual void RegisterSaverInternal(SaverBase saver)
        {
            for (int i = 0; i < m_Savers.Count; i++) {
                if (m_Savers[i].FullKey != saver.FullKey) { continue; }

                if (m_Savers[i] != saver) {
                    Debug.LogWarningFormat("Saver won't be registered because one with the same key is already registered");
                }

                return;
            }
            m_Savers.Add(saver);
        }

        /// <summary>
        /// Unregister saver components
        /// </summary>
        /// <param name="saver">The saver.</param>
        protected virtual void UnregisterSaverInternal(SaverBase saver)
        {
            m_Savers.Remove(saver);
        }

        /// <summary>
        /// Get the current save data.
        /// </summary>
        /// <returns>The save data.</returns>
        public virtual SaveData GetCurrentSaveDataInternal()
        {
            return m_SaveData;
        }

        /// <summary>
        /// Set the current save data.
        /// </summary>
        /// <param name="newSaveData">The new save data.</param>
        public virtual void SetCurrentSaveDataInternal(SaveData newSaveData)
        {
            if (newSaveData == null) { return; }
            m_SaveData = newSaveData;
        }

        /// <summary>
        /// Add save data.
        /// </summary>
        /// <param name="fullKey">The full key.</param>
        /// <param name="serializedSaveData">The serialized data.</param>
        protected virtual void AddToSaveDataInternal(string fullKey, Serialization serializedSaveData)
        {
            m_SaveData[fullKey] = serializedSaveData;
        }

        /// <summary>
        /// Save the data to the index provided.
        /// </summary>
        /// <param name="saveIndex">The save index.</param>
        protected virtual void SaveInternal(int saveIndex)
        {
            //Debug.Log("save Index" +saveIndex);

            OrderSaversByPriority();
            for (int i = 0; i < m_Savers.Count; i++) {
                m_Savers[i].Save();
            }

            SaveToDiskInternal(saveIndex);
        }

        /// <summary>
        /// Try get the save data.
        /// </summary>
        /// <param name="fullKey">The full key.</param>
        /// <param name="serializedData">The serialized data.</param>
        /// <returns>True if the save data exists.</returns>
        protected virtual bool TryGetSaverDataInternal(string fullKey, out Serialization serializedData)
        {
            if (m_SaveData == null) { Initialize(); }
            return m_SaveData.TryGetValue(fullKey, out serializedData);
        }

        /// <summary>
        /// Load the saved data from the save index provided.
        /// </summary>
        /// <param name="saveIndex">The save index.</param>
        protected virtual void LoadInternal(int saveIndex)
        {
            if (m_Saves == null || !m_Saves.ContainsKey(saveIndex)) {
                Debug.LogError($"Cannot load save at index {saveIndex}.");
                return;
            }

            m_SaveData = new SaveData(m_Saves[saveIndex]);

            OrderSaversByPriority();
            for (int i = 0; i < m_Savers.Count; i++) {
                m_Savers[i].Load();
            }
        }

        /// <summary>
        /// Delete the save from the save index.
        /// </summary>
        /// <param name="saveIndex">The save index.</param>
        protected virtual void DeleteSaveInternal(int saveIndex)
        {
            DeleteFromDiskInternal(saveIndex);
        }

        /// <summary>
        /// Get the save file path.
        /// </summary>
        /// <param name="saveIndex">The save index.</param>
        /// <returns>The save file path.</returns>
        protected virtual string GetSaveFilePath(int saveIndex)
        {
            return string.Format("{0}/{1}_{2:000}.save",
                GetSaveFolderPath(), m_SaveFileName, saveIndex);
        }

        /// <summary>
        /// Return the save folder path.
        /// </summary>
        /// <returns>The save folder path.</returns>
        protected virtual string GetSaveFolderPath()
        {
            return Application.persistentDataPath;
        }

        /// <summary>
        /// Save to disk.
        /// </summary>
        /// <param name="saveIndex">The save index.</param>
        protected virtual void SaveToDiskInternal(int saveIndex)
        {
            var saveFilePath = GetSaveFilePath(saveIndex);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(saveFilePath);

            m_SaveData.SetDateTime(DateTime.Now);
            var json = JsonUtility.ToJson(m_SaveData);

            bf.Serialize(file, json);
            file.Close();

            m_Saves[saveIndex] = new SaveData(m_SaveData);
        }

        /// <summary>
        /// Get the save data from the disk.
        /// </summary>
        /// <param name="saves">The saves dictionary.</param>
        protected virtual void GetSavesFromDisk(ref Dictionary<int, SaveData> saves)
        {
            for (int i = 0; i < m_MaxSaves; i++) {
                var saveFilePath = GetSaveFilePath(i);
                if (!File.Exists(saveFilePath)) { continue; }

                var saveData = new SaveData();

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(saveFilePath, FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), saveData);
                file.Close();

                saves.Add(i, saveData);
            }
        }

        /// <summary>
        /// Delete the save from disk.
        /// </summary>
        /// <param name="saveIndex">The save index.</param>
        private void DeleteFromDiskInternal(int saveIndex)
        {
            var saveFilePath = GetSaveFilePath(saveIndex);
            if (!File.Exists(saveFilePath)) { return; }

            File.Delete(saveFilePath);
            m_Saves.Remove(saveIndex);
        }

        /// <summary>
        /// Get the saves data.
        /// </summary>
        /// <returns>Returns all the saves.</returns>
        protected virtual IReadOnlyList<SaveData> GetSavesInternal()
        {
            for (int i = 0; i < m_SavesArray.Length; i++) {
                if (m_Saves.ContainsKey(i)) {
                    m_SavesArray[i] = m_Saves[i];
                } else { m_SavesArray[i] = null; }
            }

            return m_SavesArray;
        }

        /// <summary>
        /// Order saver components by priority.
        /// </summary>
        protected void OrderSaversByPriority()
        {
            m_Savers.Sort((x, y) => x.Priority == y.Priority ? 0 : x.Priority > y.Priority ? 1 : -1);
        }

        /// <summary>
        /// Do something when object is destroyed.
        /// </summary>
        /// <param name="obj">The object being destroyed.</param>
        protected virtual void DestroyInternal(GameObject obj)
        {
            s_Initialized = false;
        }

        /// <summary>
        /// The game has ended. Determine if the game should be auto saved.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            if (!m_AutoSaveOnApplicationQuit) { return; }

            OrderSaversByPriority();
            for (int i = 0; i < m_Savers.Count; i++) {
                if (m_Savers[i].SaveOnApplicationQuit) {
                    m_Savers[i].Save();
                }
            }

            SaveToDiskInternal(0);
        }
    }
}

