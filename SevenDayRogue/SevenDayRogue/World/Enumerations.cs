using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenDayRogue
{
    public enum TileType
    {
        None,
        Stone,
        Wood,
        Door,
        StairUp,
        StairDown,

    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    public enum WallTileType
    {
        None,
        Top,
        TopRight,
        InnerTopRight,
        Right,
        InnerBottomRight,
        BottomRight,
        Bottom,
        BottomLeft,
        InnerBottomLeft,
        Left,
        InnerTopLeft,
        TopLeft,
        Center,

    }


    public enum BulletType
    {
        Red, //damage (for burning plaque, etc)
        Blue, //Electricity (for disabling virus, etc)
        Green, //poison (for killing bacteria)
        Yellow, //radiation (for killing cancer)
        EnemyNormal,
        EnemySeeking
    }

    public enum MouseButton
    {
        None,
        LeftButton,
        RightButton,
        Wheel,
    }

    public enum EnemyMoveType
    {
        Static,
        SmallPatrol,
        LargePatrol,
        SeekPlayer,
        LineOfSight,
    }

    public enum EnemyShootType
    {
        Shooter,
        Sniper,
        Turret,
        Shotgun,
        Random,
        Spray,

    }



}
