using System.Diagnostics;
using AsteroidsMining.Entities; //FIX: Uncomment used dependencies
using AsteroidsMining.Systems;
using AsteroidsMining.Utils;

namespace AsteroidsMining
{
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
        private static int targetFPS = 60;


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
            asteroidPool = new AsteroidPool(ship, 3000);

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

                frameTimer.Restart();   //

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
                if (asteroidPool.AllAsteroidsMined())
                {
                    DisplayGameComplete();
                    break;
                }

                while (frameTimer.Elapsed.TotalSeconds < frameTimeMs)   //
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
