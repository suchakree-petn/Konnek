using System;
using Unity.Collections;
using Unity.Netcode;

namespace Konnek.KonnekLobby
{
    public struct PlayerLobby : INetworkSerializable, IEquatable<PlayerLobby>
    {
        public ulong ClientId;
        public FixedString32Bytes PlayerName;
        public bool IsReady;

        public bool Equals(PlayerLobby other)
        {
            return other.ClientId == ClientId
            && other.PlayerName == PlayerName
            && other.IsReady == IsReady;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref IsReady);
        }
    }
}