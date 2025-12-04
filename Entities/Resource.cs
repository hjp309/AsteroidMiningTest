using System;

namespace AsteroidsMining.Entities
{
    public enum ResourceType
    {
        Iron,
        Gold,
        Platinum,
        Quantum
    }

    public class Resource
    {
        public ResourceType Type { get; set; }
        public double Weight { get; set; }
        public int Quantity { get; set; }

        public Resource(ResourceType type, int quantity)
        {
            Type = type;
            Quantity = quantity;
            Weight = GetResourceWeight(type) * quantity;
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
    }
}