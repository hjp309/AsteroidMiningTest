using System.Diagnostics;
using AsteroidsMining.Entities; //FIX: Uncomment used dependencies
using AsteroidsMining.Systems;
using AsteroidsMining.Utils;

namespace AsteroidsMining
{
    public class AsteroidPool
    {
        private Ship ship;
        public int currentAsteroidCount;
        public int targetAsteroidCount;
        public List<Asteroid> asteroids;
        private Random randomSeed;
        public event Action<Asteroid> OnMined;

        // With ship for gameplay.
        public AsteroidPool(Ship ship, int poolSize, int totalAsteroids)
        {
            this.ship = ship;
            targetAsteroidCount = totalAsteroids;
            currentAsteroidCount = 0;
            randomSeed = new Random();
            asteroids = new List<Asteroid>();

            for (int i = 0; i < poolSize; i++)
                GenerateAsteroid();
                
            Console.WriteLine($"Generated {poolSize} asteroids out of {totalAsteroids}");
        }

        // Without ship for testing.
        public AsteroidPool(int poolSize, int totalAsteroids)
        {
            targetAsteroidCount = totalAsteroids;
            currentAsteroidCount = 0;
            randomSeed = new Random();
            asteroids = new List<Asteroid>();

            for (int i = 0; i < poolSize; i++)
                GenerateAsteroid();
                
            Console.WriteLine($"Generated {poolSize} asteroids out of {totalAsteroids}");
        }

        // Modified to generate one at a time, added positional parameters to adjust spawn placement.
        public void GenerateAsteroid(double X = 0.0, double Y = 0.0){
            if (currentAsteroidCount == targetAsteroidCount)
                return;

            double x = MathUtils.GetRandomDouble(50, 950);
            double y = MathUtils.GetRandomDouble(50, 950);

            while (MathUtils.CalculateDistance(x, y, X, Y) < 30)
            {
                x = MathUtils.GetRandomDouble(50, 950);
                y = MathUtils.GetRandomDouble(50, 950);
            }
            
            // Generate resource type with bias toward common resources
            ResourceType resourceType;
            int randomValue = randomSeed.Next(100);
            if (randomValue < 50)
                resourceType = ResourceType.Iron;
            else if (randomValue < 75)
                resourceType = ResourceType.Gold;
            else if (randomValue < 90)
                resourceType = ResourceType.Platinum;
            else
                resourceType = ResourceType.Quantum;
            
            int quantity = randomSeed.Next(1, 10);
            
            var asteroid = new Asteroid(x, y, resourceType, quantity); //FIX: Missing quantity parameter
            
            asteroid.OnMined += HandleAsteroidMined;
            
            asteroids.Add(asteroid);
            currentAsteroidCount++;
        }

        public int GetRemainingAsteroidCount()
        {
            return targetAsteroidCount - currentAsteroidCount; 
        }

        public bool AreAllAsteroidsCleared()
        {
            return currentAsteroidCount == targetAsteroidCount;
        }

        private void HandleAsteroidMined(Asteroid asteroid)
        {
            // FIX: Object pooling. Remove and replace asteroid. Unsubscribe event subscription.
            asteroid.OnMined -= HandleAsteroidMined;
            asteroids.Remove(asteroid);
            if (currentAsteroidCount < targetAsteroidCount) 
                GenerateAsteroid(ship.X, ship.Y);
        }
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
                if (asteroidPool.AreAllAsteroidsCleared())
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
