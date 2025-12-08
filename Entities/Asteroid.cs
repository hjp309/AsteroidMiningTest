namespace AsteroidsMining.Entities
{
    public class Asteroid
    {
        public double X { get; set; }
        public double Y { get; set; }
        public ResourceType ResourceType { get; set; }
        public int ResourceQuantity { get; set; }
        public double MiningDifficulty { get; set; }

        public event Action<Asteroid> OnMined;

        public Asteroid(double x, double y, ResourceType resourceType, int quantity)
        {
            X = x;
            Y = y;
            ResourceType = resourceType;
            ResourceQuantity = quantity;
            MiningDifficulty = GetMiningDifficulty(resourceType);
        }

        private double GetMiningDifficulty(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Iron:
                    return 1.0;
                case ResourceType.Gold:
                    return 2.0;
                case ResourceType.Platinum:
                    return 3.5;
                case ResourceType.Quantum:
                    return 5.0;
                default:
                    return 1.0;
            }
        }

        public Resource Mine()
        {
            var resource = new Resource(ResourceType, ResourceQuantity);

            OnMined?.Invoke(this);

            return resource;
        }

        public void RespawnAsteroid(double x, double y, ResourceType resourceType, int quantity)
        {
            X = x;
            Y = y;
            ResourceType = resourceType;
            ResourceQuantity = quantity;
            MiningDifficulty = GetMiningDifficulty(resourceType);
        }

        public double GetDistanceTo(double x, double y)
        {
            return Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
        }

        public void ClearEventHandler()
        {
            OnMined = null;
        }
    }
}