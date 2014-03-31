using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenDayRogue
{
    public class GameConstants
    {
        public static int LevelHeight = 100;
        public static int LevelWidth = 100;

        public static int TileWidth = 25;
        public static int TileHeight = 25;

        public static float playerSpeed = 10000;
        public static int playerShootSpeed = 750;

        public static float enemyPercent = .005f;  //percent that an enemy will be in a floor square

        public static int slowEnemyShootSpeed = 50;
        public static int medEnemyShootSpeed = 100;
        public static int fastEnemyShootSpeed = 500;
        public static int snipeEnemyShootSpeed = 1000;

        public static int slowEnemySpeed;
        public static int medEnemySpeed;
        public static int fastEnemyspeed;
        public static int vFastEnemySpeed = 10000;
    }
}
