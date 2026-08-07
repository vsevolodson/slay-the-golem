using System;
using System.IO;
using UnityEngine;

namespace Game.Unity.Save
{
    public sealed class RunSaveStorage
    {
        private readonly string _path;

        public RunSaveStorage(string fileName)
        {
            _path = Path.Combine(Application.persistentDataPath, fileName);
        }

        public void Save(string json)
        {
            try
            {
                File.WriteAllText(_path, json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"could not write the save file: {exception.Message}");
            }
        }

        public bool TryLoad(out string json)
        {
            json = null;

            try
            {
                if (!File.Exists(_path))
                    return false;

                json = File.ReadAllText(_path);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"could not read the save file: {exception.Message}");

                return false;
            }

            return true;
        }
    }
}