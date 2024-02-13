using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayedPosition : INetworkSerializable, IEquatable<PlayedPosition>
{
    public Vector3 Value;

    public PlayedPosition(Vector3 value)
    {
        Value = value;
    }
    public bool Equals(PlayedPosition other)
    {
        return Value == other.Value;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Value);
    }
}

