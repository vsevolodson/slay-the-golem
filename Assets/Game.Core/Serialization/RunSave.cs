using Game.Core.State;
using Newtonsoft.Json;

namespace Game.Core.Serialization
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RunSave
    {
        [JsonProperty] public int Version { get; private set; }
        [JsonProperty] public RunState State { get; private set; }

        internal RunSave(int version, RunState state)
        {
            Version = version;
            State = state;
        }

        [JsonConstructor]
        private RunSave()
        {
        }
    }
}