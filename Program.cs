using System.Diagnostics;
using AsteroidsMining.Entities; //FIX: Uncomment used dependencies
using AsteroidsMining.Systems;
using AsteroidsMining.Tests;
using AsteroidsMining.Utils;

namespace AsteroidsMining
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


    class Program
    {
        private static Ship ship;
        private static MiningSystem miningSystem;
        private static MovementSystem movementSystem;
        private static CargoSystem cargoSystem;
        private static RenderSystem renderSystem;
        private static PerformanceMonitor performanceMonitor;
        private static AsteroidPool asteroidPool;

        private static Stopwatch frameTimer;

        private static bool gameRunning = true;

        static void Main(string[] args)
        {
            Console.WriteLine("=== ASTEROIDS MINING SIMULATION ===");
            Console.WriteLine("Initializing game world...");

            try
            {
                InitializeGame();
                RunGameLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Game crashed: {ex}");   // Altered temporarily for error clarity.
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static void InitializeGame()
        {
            // Initialize performance monitoring
            performanceMonitor = new PerformanceMonitor();
            
            // Create ship at base (0, 0)
            ship = new Ship(0, 0, 15.0, 100.0);
            
            // Generate asteroids
            // FIX: Modified for gameplay feel, spawns only 100 at a time.
            asteroidPool = new AsteroidPool(ship, 100, 3000);
            
            // Initialize systems
            miningSystem = new MiningSystem(ship, asteroidPool);
            movementSystem = new MovementSystem(ship, asteroidPool);
            cargoSystem = new CargoSystem(ship);
            renderSystem = new RenderSystem(ship, asteroidPool);
            
            performanceMonitor.StartGame(); // FIX: BeginGame -> StartGame

            frameTimer = Stopwatch.StartNew();

            Console.WriteLine("Game initialized successfully!");
            Thread.Sleep(1000); // Pause to show message (Adjusted time for faster iterations)
        }

        private static void RunGameLoop()
        {
            const int targetFPS = 60;
            const double frameTimeMs = 1.0 / targetFPS;
            
            Console.Clear();
            Console.WriteLine("Starting game loop...");
            Thread.Sleep(1000);
            
            while (gameRunning)
            {
                performanceMonitor.StartLoop();

                double dt = frameTimer.Elapsed.TotalSeconds;
                frameTimer.Restart();
                
                // Update game state
                UpdateGame();
                
                // Render current state every 30 frames to reduce spam
                if (performanceMonitor.frameCount % 30 == 0)    //FIX: Make frameCount public
                {
                    renderSystem.DisplayGameState();
                    renderSystem.DisplayMiniMap();
                    renderSystem.DisplayNearbyAsteroids();
                    performanceMonitor.DisplayPerformanceStats();
                }
                
                // Check if game is complete
                if (asteroidPool.AllAsteroidsSpawned())
                {
                    DisplayGameComplete();
                    break;
                }
                
                while (frameTimer.Elapsed.TotalSeconds < frameTimeMs)
                    Thread.SpinWait(1);
                
                performanceMonitor.EndLoop();
                
                if (performanceMonitor.GetCurrentFPS() % 300 == 0)
                {
                    performanceMonitor.CheckMemoryPressure();
                }
                
                // Allow breaking out of loop for testing
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        gameRunning = false;
                    }
                }
            }
        }

        private static void UpdateGame()
        {
            // Determine ship behavior based on cargo state
            if (cargoSystem.ShouldReturnToBase())
            {
                movementSystem.ReturnToBase();
                
                // Check if we're at base and unload
                if (cargoSystem.IsAtBase()) // FIX: Unnecessary helper function
                {
                    cargoSystem.ProcessCargoDelivery();
                }
            }
            else
            {
                // Move toward nearest asteroid
                movementSystem.MoveToNearestAsteroid();
                
                // Try to mine if in range
                miningSystem.ProcessMining();
            }
            
            if (performanceMonitor.IsPerformanceDegrading())
            {
                Console.WriteLine("WARNING: Performance degrading!");
                performanceMonitor.LogCustomMetric("PerformanceDegradation", 1.0);
            }
        }

        private static void DisplayGameComplete()
        {
            Console.Clear();
            Console.WriteLine("=== GAME COMPLETE! ===");
            Console.WriteLine($"All {asteroidPool.targetAsteroidCount} asteroids have been cleared!");
            Console.WriteLine($"Total Distance Traveled: {ship.TotalDistanceTraveled:F1}");
            Console.WriteLine($"Total Trips to Base: {cargoSystem.GetTotalTrips()}");
            Console.WriteLine($"Resources Delivered: {cargoSystem.GetTotalResourcesDelivered()}");
            Console.WriteLine($"Efficiency: {cargoSystem.CalculateCargoEfficiency():F3} resources/distance");
            
            performanceMonitor.DisplayPerformanceStats();
            
            renderSystem.DisplayMiniMap();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
