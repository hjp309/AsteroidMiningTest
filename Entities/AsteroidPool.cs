using AsteroidsMining.Utils;

namespace AsteroidsMining.Entities
{
    public class AsteroidPool
    {
        private readonly Ship ship;

        public int totalSpawnedAsteroids;   // Total created EVER
        public int targetAsteroidCount;     // Target Total
        public List<Asteroid> asteroids;    // Active asteroids

        private Random random = new Random();

        public AsteroidPool(Ship ship, int initialPoolSize, int totalToSpawn)
        {
            this.ship = ship;
            this.targetAsteroidCount = totalToSpawn;
            this.totalSpawnedAsteroids = 0;
            this.asteroids = new List<Asteroid>();

            // Pre-generate initial pool
            for (int i = 0; i < initialPoolSize; i++)
                SpawnNewAsteroid(0, 0);
        }

        // Initial asteroid spawn
        private void SpawnNewAsteroid(double X, double Y)
        {
            if (totalSpawnedAsteroids >= targetAsteroidCount)
                return;

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
            totalSpawnedAsteroids++;
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

            // Keep the active pool filled (object pooling)
            if (totalSpawnedAsteroids < targetAsteroidCount)
            {
                // Reuse asteroid
                RespawnAsteroid(asteroid);
            }
        }

        // Respawn and resubscribe logic.
        private void RespawnAsteroid(Asteroid asteroid)
        {
            double x = MathUtils.GetRandomDouble(50, 950);
            double y = MathUtils.GetRandomDouble(50, 950);

            while (MathUtils.CalculateDistance(x, y, ship.X, ship.Y) < 30)
            {
                x = MathUtils.GetRandomDouble(50, 950);
                y = MathUtils.GetRandomDouble(50, 950);
            }

            var type = GenerateResourceType();
            var qty = random.Next(1, 10);

            asteroid.RespawnAsteroid(x, y, type, qty);
            asteroid.OnMined += HandleAsteroidMined;
        }

        public bool AllAsteroidsSpawned()
            => totalSpawnedAsteroids >= targetAsteroidCount;

        public int GetRemainingAsteroidCount()
            => targetAsteroidCount - totalSpawnedAsteroids;
    }
}