using System;
using Newtonsoft.Json;

namespace Game.Core.Rng
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class XorShiftRng
    {
        private const ulong FallbackState = 0x9E3779B97F4A7C15;

        [JsonProperty] private ulong _state;

        internal XorShiftRng(ulong seed)
        {
            _state = seed == 0 ? FallbackState : seed;
        }

        [JsonConstructor]
        private XorShiftRng()
        {
        }

        internal ulong NextUInt64()
        {
            _state ^= _state << 13;
            _state ^= _state >> 7;
            _state ^= _state << 17;

            return _state;
        }

        internal int Next(int exclusiveMax)
        {
            if (exclusiveMax <= 0)
                throw new ArgumentOutOfRangeException(nameof(exclusiveMax), "upper bound must be positive");

            return (int)(NextUInt64() % (ulong)exclusiveMax);
        }
    }
}