#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Direction : byte
{
    None = 0,
    Right = 1 << 0,
    Up = 1 << 1,
    Forward = 1 << 2,
    Left = 1 << 3,
    Down = 1 << 4,
    Back = 1 << 5,

    UpRight = Up | Right,
    UpLeft = Up | Left,
    UpForward = Up | Forward,
    UpBack = Up | Back,
    DownRight = Down | Right,
    DownLeft = Down | Left,
    DownForward = Down | Forward,
    DownBack = Down | Back,
    ForwardRight = Forward | Right,
    ForwardLeft = Forward | Left,
    BackRight = Back | Right,
    BackLeft = Back | Left,

    UpLeftForward = UpLeft | Forward,
    UpLeftBack = UpLeft | Back,
    UpRightForward = UpRight | Forward,
    UpRightBack = UpRight | Back,
    DownLeftForward = DownLeft | Forward,
    DownLeftBack = DownLeft | Back,
    DownRightForward = DownRight | Forward,
    DownRightBack = DownRight | Back,
}

public static class DirectionClass
{
    private readonly static Vector3Int[] _simpleDirections = new Vector3Int[6]
    {
        Vector3Int.right,
        Vector3Int.up,
        Vector3Int.forward,
        Vector3Int.left,
        Vector3Int.down,
        Vector3Int.back
    };

    //return not normalized vector
    public static Vector3Int ToVector(this Direction direction)
    {
        Vector3Int result = Vector3Int.zero;
        for(int i = 0; i < 6; i++)
        {
            if((direction & (Direction)(1 << i)) != 0)
            {
                result += _simpleDirections[i];
            }
        }
        return result;
    }

    public static Direction Normalize(this Direction direction)
    {
        Direction result = Direction.None;
        foreach(var simple in direction.ToSimples())
            result |= simple;
        return result;
    }

    public static List<Direction> ToSimples(this Direction direction)
    {
        List<Direction> result = new();
        for(int i = 0; i < 3; i++)
        {
            Direction straight = (Direction)(1 << i);
            Direction inversed = (Direction)(1 << (i + 3));
            bool hasStrait = (direction & straight) != 0;
            bool hasInversed = (direction & inversed) != 0;

            if(hasStrait)
            {
                if(hasInversed == false)
                {
                    result.Add(straight);
                }
            }
            else if(hasInversed)
            {
                result.Add(inversed);
            }
        }
        return result;
    }

    private static Direction InverseSimple(Direction direction) =>
        (Direction)(((int)direction & 15) == 0 ? ((int)direction << 3) : ((int)direction >> 3));

    public static Direction Inverse(this Direction direction)
    {
        Direction result = Direction.None;
        foreach(Direction simple in direction.ToSimples())
            result |= InverseSimple(simple);

        return result;
    }

    private static Direction ExcludeSimpleDirection(this Direction direction, Direction simpleDirectionToExclude)
    {
        return direction & (~simpleDirectionToExclude);
    }

    private static Direction InverseBySimple(this Direction direction, Direction simpleDirestion)
    {
        Direction straight = simpleDirestion;
        Direction inversed = simpleDirestion.Inverse();


        bool hasStrait = (simpleDirestion & straight) != 0;
        bool hasInversed = (simpleDirestion & inversed) != 0;

        Direction result = direction.ExcludeSimpleDirection(simpleDirestion).ExcludeSimpleDirection(inversed);

        if(hasStrait)
        {
            if(hasInversed == false)
            {
                result |= straight;
            }
        }
        else if(hasInversed)
        {
            result |= inversed;
        }

        return result;
    }

    public static Direction Inverse(this Direction direction, Direction directionsToInverse)
    {
        Direction result = Direction.None;

        foreach(Direction simple in directionsToInverse.ToSimples())
            result |= direction.InverseBySimple(simple);

        return result;
    }
}
#nullable disable

