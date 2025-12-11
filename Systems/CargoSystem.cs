using System.Text;
using AsteroidsMining.Entities;

namespace AsteroidsMining.Systems
{
    public class CargoSystem
    {
        private Ship ship;
        private int totalResourcesDelivered;
        private int totalTripsToBase;

        public static List<Resource> DeliveredResources = new List<Resource>();

        public CargoSystem(Ship ship)
        {
            this.ship = ship;
            totalResourcesDelivered = 0;
            totalTripsToBase = 0;
        }

        public bool ShouldReturnToBase()
        {
            return ship.IsCargoFull();  //FIX: Redundant and different logic. (If >=90%, cargo is pretty much full)
                                        //EDGE: Cargo may exceed 100% from <90%.
        }

        public void ProcessCargoDelivery()
        {
            if (IsAtBase())
            {
                foreach (var resource in ship.CargoHold)
                {
                    DeliveredResources.Add(resource);   //FIX: Unnecessary instancing of same resource.

                    var logMessage = new StringBuilder();
                    for (int i = 0; i < resource.Quantity; i++)
                    {
                        logMessage.Append($"Delivered {resource.Type} unit {i + 1}, ");
                    }
                    Console.WriteLine(logMessage);
                }

                totalResourcesDelivered += ship.CargoHold.Count;
                totalTripsToBase++;

                ship.UnloadCargo();
            }
        }

        public bool IsAtBase()  // FIX: Make method public
        {
            return ship.X == 0.0 && ship.Y == 0.0;
        }

        public double CalculateCargoEfficiency()
        {
            double totalWeight = 0;
            foreach (Resource resource in DeliveredResources)
            {
                totalWeight += resource.Weight; // FIX: Redundant weight calculation.
            }

            if (ship.TotalDistanceTraveled == 0) return 0;

            return totalWeight / ship.TotalDistanceTraveled;
        }

        public int GetTotalTrips()
        {
            return totalTripsToBase;
        }

        public int GetTotalResourcesDelivered()
        {
            return totalResourcesDelivered;
        }
    }
}