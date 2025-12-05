using System;
using System.Collections.Generic;
using System.Linq;

namespace AsteroidsMining.Entities
{
    public class Ship
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Speed { get; set; }
        public double CargoCapacity { get; set; }
        public double MiningRange { get; set; }
        public double TotalDistanceTraveled { get; set; }

        public List<Resource> CargoHold { get; set; }

        public Ship(double x, double y, double speed = 10.0, double cargoCapacity = 100.0)
        {
            X = x;
            Y = y;
            Speed = speed;
            CargoCapacity = cargoCapacity;
            MiningRange = 5.0;
            CargoHold = new List<Resource>();
            TotalDistanceTraveled = 0.0;
        }

        public void MoveTo(double targetX, double targetY)
        {
            double distance = Math.Sqrt((targetX - X) * (targetX - X) + (targetY - Y) * (targetY - Y));
            TotalDistanceTraveled += distance;
            X = targetX;
            Y = targetY;
        }

        public bool CanMine(Asteroid asteroid)
        {   
            double distance = asteroid.GetDistanceTo(X, Y);
            return distance <= MiningRange;
        }

        public bool AddResource(Resource resource)
        {
            double currentWeight = GetCurrentCargoWeight(); // FIX: Repeated line.
            
            if (currentWeight + resource.Weight <= CargoCapacity)
            {
                CargoHold.Add(resource);
                return true;
            }
            return false;
        }

        public void UnloadCargo()
        {
            CargoHold.Clear();
        }

        public double GetCurrentCargoWeight()
        {
            return CargoHold.Sum(r => r.Weight);
        }

        public bool IsCargoFull()
        {
            return GetCurrentCargoWeight() >= CargoCapacity * 0.9; // 90% full
        }
    }
}