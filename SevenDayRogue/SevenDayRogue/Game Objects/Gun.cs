using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenDayRogue
{
    public class Gun
    {
        public string name;
        public int levelReq;
        public int fireCost;
        public float fireRate;
        public int damage;
        public int speed;

        public int buyCost;
        public int sellCost;

        //sprite

        //special ability

        public Gun(string name, int levelReq, int fireCost, float fireRate, int dmg, int speed, int buyCost, int sellCost)
        {
            this.name = name;
            this.levelReq = levelReq;
            this.fireCost = fireCost;
            this.fireRate = fireRate;
            this.damage = dmg;
            this.speed = speed;
            this.buyCost = buyCost;
            this.sellCost = sellCost;
        }

        //Gun Factory

        public static Gun getPistol()
        {
            return new Gun("Basic Pistol", 1, 1, .5f, 10, 500, 0, 0);
        }

        public static Gun getMiniGun()
        {
            return new Gun("Minigun", 1, 1, .05f, 10, 1000, 0, 0);
        }

    }
}
