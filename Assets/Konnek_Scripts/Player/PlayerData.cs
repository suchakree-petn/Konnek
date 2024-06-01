using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public FixedString32Bytes PlayerName;
    public FixedString32Bytes PlayerId;
    public ulong ClientId;

    public bool Equals(PlayerData other)
    {
        return other.PlayerName == PlayerName
        && other.ClientId == ClientId
        && other.PlayerId == PlayerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref ClientId);
    }
}
