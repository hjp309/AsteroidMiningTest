using System;
using System.Collections.Generic;
using System.Diagnostics;
using AsteroidsMining.Entities;
using AsteroidsMining.Systems;
using AsteroidsMining.Utils;

namespace AsteroidsMining.Tests
{
    public class PerformanceTests
    {
        public static void RunBasicPerformanceTest()
        {
            Console.WriteLine("=== PERFORMANCE TEST ===");
            
            // Test asteroid generation performance
            var stopwatch = Stopwatch.StartNew();
            var asteroids = GenerateTestAsteroids(1000);
            stopwatch.Stop();
            Console.WriteLine($"Generated 1000 asteroids in {stopwatch.ElapsedMilliseconds}ms");
            
            // Test distance calculation performance
            var ship = new Ship(0, 0);
            stopwatch.Restart();
            
            for (int i = 0; i < 1000; i++)
            {
                foreach (var asteroid in asteroids)
                {
                    double distance = asteroid.GetDistanceTo(ship.X, ship.Y);
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine($"1M distance calculations: {stopwatch.ElapsedMilliseconds}ms");
            
            // Test mining system performance
            var miningSystem = new MiningSystem(ship, asteroids);
            stopwatch.Restart();
            
            for (int i = 0; i < 100; i++)
            {
                var nearest = miningSystem.FindNearestAsteroid();
                if (nearest != null) break;
            }
            
            stopwatch.Stop();
            Console.WriteLine($"100 nearest asteroid searches: {stopwatch.ElapsedMilliseconds}ms");
        }

        private static List<Asteroid> GenerateTestAsteroids(int count)
        {
            var asteroids = new List<Asteroid>();
            var random = new Random();
            
            for (int i = 0; i < count; i++)
            {
                double x = MathUtils.GetRandomDouble(0, 1000);
                double y = MathUtils.GetRandomDouble(0, 1000);
                var type = (ResourceType)random.Next(4);
                int quantity = random.Next(1, 10);
                
                asteroids.Add(new Asteroid(x, y, type, quantity));
            }
            
            return asteroids;
        }

        public static void TestMemoryUsage()
        {
            Console.WriteLine("=== MEMORY USAGE TEST ===");
            
            long initialMemory = GC.GetTotalMemory(true);
            Console.WriteLine($"Initial memory: {initialMemory / 1024}KB");
            
            // Create objects that will cause memory issues
            var asteroids = GenerateTestAsteroids(5000);
            var ship = new Ship(500, 500);
            var renderSystem = new RenderSystem(ship, asteroids);
            
            // Simulate memory leaks
            for (int i = 0; i < 100; i++)
            {
                foreach (var asteroid in asteroids)
                {
                    asteroid.OnMined += (a) => Console.WriteLine("Memory leak!");
                }
            }
            
            long currentMemory = GC.GetTotalMemory(false);
            Console.WriteLine($"Memory after test: {currentMemory / 1024}KB");
            Console.WriteLine($"Memory increase: {(currentMemory - initialMemory) / 1024}KB");
        }
    }
}