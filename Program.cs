using System;
using System.Collections.Generic;
using System.Threading;
using AsteroidsMining.Entities;
using AsteroidsMining.Systems;
using AsteroidsMining.Utils;

namespace AsteroidsMining
{
    class Program
    {
        private static Ship ship;
        private static List<Asteroid> asteroids;
        private static MiningSystem miningSystem;
        private static MovementSystem movementSystem;
        private static CargoSystem cargoSystem;
        private static RenderSystem renderSystem;
        private static PerformanceMonitor performanceMonitor;

        private static bool gameRunning = true;
        private static int targetAsteroidCount = 1000;

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
                Console.WriteLine($"Game crashed: {ex.Message}");
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
            GenerateAsteroids();

            // Initialize systems
            miningSystem = new MiningSystem(ship, asteroids);
            movementSystem = new MovementSystem(ship, asteroids);
            cargoSystem = new CargoSystem(ship);
            renderSystem = new RenderSystem(ship, asteroids);

            performanceMonitor.StartGame();

            Console.WriteLine($"Generated {asteroids.Count} asteroids");
            Console.WriteLine("Game initialized successfully!");
            Thread.Sleep(2000); // Pause to show message
        }

        private static void GenerateAsteroids()
        {
            asteroids = new List<Asteroid>();
            var random = new Random();

            for (int i = 0; i < targetAsteroidCount; i++)
            {
                double x = MathUtils.GetRandomDouble(50, 950);
                double y = MathUtils.GetRandomDouble(50, 950);

                while (MathUtils.CalculateDistance(x, y, 0, 0) < 30)
                {
                    x = MathUtils.GetRandomDouble(50, 950);
                    y = MathUtils.GetRandomDouble(50, 950);
                }

                // Generate resource type with bias toward common resources
                ResourceType resourceType;
                int randomValue = random.Next(100);
                if (randomValue < 50)
                    resourceType = ResourceType.Iron;
                else if (randomValue < 75)
                    resourceType = ResourceType.Gold;
                else if (randomValue < 90)
                    resourceType = ResourceType.Platinum;
                else
                    resourceType = ResourceType.Quantum;

                int quantity = random.Next(1, 10);

                var asteroid = new Asteroid(x, y, resourceType, quantity);

                asteroid.OnMined += HandleAsteroidMined;

                asteroids.Add(asteroid);
            }
        }

        private static void RunGameLoop()
        {
            const int targetFPS = 60;
            const int frameTimeMs = 1000 / targetFPS;

            Console.Clear();
            Console.WriteLine("Starting game loop...");
            Thread.Sleep(1000);

            while (gameRunning)
            {
                performanceMonitor.StartLoop();

                // Update game state
                UpdateGame();

                // Render current state every 30 frames to reduce spam
                if (performanceMonitor.frameCount % 30 == 0)
                {
                    renderSystem.DisplayGameState();
                    renderSystem.DisplayNearbyAsteroids();
                    performanceMonitor.DisplayPerformanceStats();
                }

                // Check if game is complete
                if (AreAllAsteroidsCleared())
                {
                    DisplayGameComplete();
                    break;
                }

                performanceMonitor.EndLoop();

                Thread.Sleep(frameTimeMs);

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
                if (IsShipAtBase())
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

        private static bool IsShipAtBase()
        {
            return cargoSystem.IsAtBase();
        }

        private static bool AreAllAsteroidsCleared()
        {
            foreach (var asteroid in asteroids)
            {
                if (!asteroid.IsDepleted)
                    return false;
            }
            return true;
        }

        private static void DisplayGameComplete()
        {
            Console.Clear();
            Console.WriteLine("=== GAME COMPLETE! ===");
            Console.WriteLine($"All {targetAsteroidCount} asteroids have been cleared!");
            Console.WriteLine($"Total Distance Traveled: {ship.TotalDistanceTraveled:F1}");
            Console.WriteLine($"Total Trips to Base: {cargoSystem.GetTotalTrips()}");
            Console.WriteLine($"Resources Delivered: {cargoSystem.GetTotalResourcesDelivered()}");
            Console.WriteLine($"Efficiency: {cargoSystem.CalculateCargoEfficiency():F3} resources/distance");

            performanceMonitor.DisplayPerformanceStats();

            renderSystem.DisplayMiniMap();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void HandleAsteroidMined(Asteroid asteroid)
        {
            var message = $"Mined asteroid at ({asteroid.X}, {asteroid.Y})";
            Console.WriteLine(message);

            MiningSystem.OnMiningComplete += (msg) => Console.WriteLine($"Mining: {msg}");
        }

    }
}
