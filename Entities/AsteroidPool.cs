using AsteroidsMining.Utils;

namespace AsteroidsMining.Entities
{
    public class AsteroidPool
    {
        private readonly Ship ship;
        public int targetAsteroidCount;     // Target Total
        public int totalMined;
        public List<Asteroid> asteroids;    // Active asteroids

        private Random random = new Random();

        public AsteroidPool(Ship ship, int totalToSpawn)
        {
            this.ship = ship;
            this.targetAsteroidCount = totalToSpawn;
            this.totalMined = 0;
            this.asteroids = new List<Asteroid>();

            // Pre-generate initial pool
            for (int i = 0; i < totalToSpawn; i++)
                SpawnNewAsteroid(ship.X, ship.Y);
        }

        // Initial asteroid spawn
        private void SpawnNewAsteroid(double X, double Y)
        {
            double x = MathUtils.GetRandomDouble(50, 950);
            double y = MathUtils.GetRandomDouble(50, 950);

            // Avoid spawning near ship or previous asteroid position
            while (MathUtils.CalculateDistance(x, y, X, Y) < 30)
            {
                x = MathUtils.GetRandomDouble(50, 950);
                y = MathUtils.GetRandomDouble(50, 950);
            }

            // Resource type (weighted)
            ResourceType type = GenerateResourceType();
            int qty = random.Next(1, 10);

            // Create & track
            Asteroid asteroid = new Asteroid(x, y, type, qty);

            asteroid.OnMined += HandleAsteroidMined; // attach pooled handler

            asteroids.Add(asteroid);
        }

        // Moved random resource definition for respawn and initial spawn
        private ResourceType GenerateResourceType()
        {
            int roll = random.Next(100);

            if (roll < 50) return ResourceType.Iron;
            if (roll < 75) return ResourceType.Gold;
            if (roll < 90) return ResourceType.Platinum;
            return ResourceType.Quantum;
        }

        // Garbage collection and reusing asteroids
        private void HandleAsteroidMined(Asteroid asteroid)
        {
            asteroid.OnMined -= HandleAsteroidMined;
            asteroid.ClearEventHandler();
            asteroids.Remove(asteroid);
            totalMined++;
        }

        public bool AllAsteroidsMined()
            => totalMined >= targetAsteroidCount;

        public int GetRemainingAsteroidCount()
            => asteroids.Count - totalMined;
    }
}