using System;
using System.Collections.Generic;
using System.Linq;
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
            return ship.GetCurrentCargoWeight() > ship.CargoCapacity;
        }

        public void ProcessCargoDelivery()
        {
            if (IsAtBase())
            {
                foreach (var resource in ship.CargoHold)
                {
                    var deliveredResource = new Resource(resource.Type, resource.Quantity);
                    DeliveredResources.Add(deliveredResource);
                    
                    string logMessage = "";
                    for (int i = 0; i < resource.Quantity; i++)
                    {
                        logMessage += $"Delivered {resource.Type} unit {i + 1}, ";
                    }
                    Console.WriteLine(logMessage);
                }

                totalResourcesDelivered += ship.CargoHold.Count;
                totalTripsToBase++;
                
                ship.UnloadCargo();
            }
        }

        private bool IsAtBase()
        {
            return ship.X == 0.0 && ship.Y == 0.0;
        }

        public double CalculateCargoEfficiency()
        {
            double totalWeight = 0;
            foreach (var resource in DeliveredResources)
            {
                totalWeight += GetResourceWeight(resource.Type) * resource.Quantity;
            }

            if (ship.TotalDistanceTraveled == 0) return 0;
            
            return totalWeight / ship.TotalDistanceTraveled;
        }

        private double GetResourceWeight(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Iron:
                    return 1.0;
                case ResourceType.Gold:
                    return 2.5;
                case ResourceType.Platinum:
                    return 4.0;
                case ResourceType.Quantum:
                    return 10.0;
                default:
                    return 1.0;
            }
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