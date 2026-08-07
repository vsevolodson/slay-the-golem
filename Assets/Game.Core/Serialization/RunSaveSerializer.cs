using System;
using Game.Core.State;
using Newtonsoft.Json;

namespace Game.Core.Serialization
{
    public static class RunSaveSerializer
    {
        public const int CurrentVersion = 2;

        public static string Save(RunState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            return CoreJson.Serialize(new RunSave(CurrentVersion, state));
        }

        public static bool TryLoad(string json, out RunState state)
        {
            state = null;

            if (string.IsNullOrWhiteSpace(json))
                return false;

            RunSave save;

            try
            {
                save = CoreJson.Deserialize<RunSave>(json);
            }
            catch (JsonException)
            {
                return false;
            }

            if (save == null || save.Version != CurrentVersion || save.State == null)
                return false;

            state = save.State;

            return true;
        }
    }
}