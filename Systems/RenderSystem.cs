using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsteroidsMining.Entities;

namespace AsteroidsMining.Systems
{
    public class RenderSystem
    {
        private Ship ship;
        private AsteroidPool asteroidPool;

        public static string LogHistory = "";

        public RenderSystem(Ship ship, AsteroidPool asteroidPool)
        {
            this.ship = ship;
            this.asteroidPool = asteroidPool;
        }

        public void DisplayGameState()
        {
            Console.Clear();
            
            string output = "";
            output += "=== ASTEROIDS MINING GAME ===\n";
            output += $"Ship Position: ({ship.X:F1}, {ship.Y:F1})\n";
            output += $"Cargo: {ship.GetCurrentCargoWeight():F1}/{ship.CargoCapacity:F1}\n";
            output += $"Distance Traveled: {ship.TotalDistanceTraveled:F1}\n";
            
            output += $"Remaining Asteroids: {asteroidPool.GetRemainingAsteroidCount()}\n";
            
            var cargoStatus = new StringBuilder();
            foreach (var resource in ship.CargoHold)
            {
                cargoStatus.Append($"{resource.Type}: {resource.Quantity}, ");
            }
            output += $"Cargo Contents: {cargoStatus.ToString()}\n";
            
            output += "\n";
            
            Console.WriteLine(output);
            
            // FIX: Concatenating string for no reason. Saving for now in case there's a purpose.
            //LogHistory += output;
        }

        public void DisplayNearbyAsteroids(double range = 50.0)
        {
            var nearbyAsteroids = new List<Asteroid>();
            foreach (var asteroid in asteroidPool.asteroids)
            {
                double distance = asteroid.GetDistanceTo(ship.X, ship.Y);
                if (distance <= range)
                {
                    nearbyAsteroids.Add(asteroid);
                }
            }

            Console.WriteLine("Nearby Asteroids:");
            var sortedAsteroids = nearbyAsteroids.OrderBy(a => a.GetDistanceTo(ship.X, ship.Y)).ToList();
            
            for (int i = 0; i < Math.Min(5, sortedAsteroids.Count); i++)
            {
                var asteroid = sortedAsteroids[i];
                double distance = asteroid.GetDistanceTo(ship.X, ship.Y);
                
                string asteroidInfo = "";
                asteroidInfo += $"  [{i+1}] ";
                asteroidInfo += $"Type: {asteroid.ResourceType} ";
                asteroidInfo += $"Qty: {asteroid.ResourceQuantity} ";
                asteroidInfo += $"Pos: ({asteroid.X:F1}, {asteroid.Y:F1}) ";
                asteroidInfo += $"Dist: {distance:F1}";
                
                Console.WriteLine(asteroidInfo);
            }
        }

        public void DisplayMiniMap()
        {
            const int mapWidth = 40;
            const int mapHeight = 20;
            const int worldSize = 1000;

            char[,] map = new char[mapHeight, mapWidth];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    map[y, x] = ' ';
                }
            }

            foreach (var asteroid in asteroidPool.asteroids)
            {
                int mapX = (int)((asteroid.X / worldSize) * mapWidth);
                int mapY = (int)((asteroid.Y / worldSize) * mapHeight);
                
                if (mapX >= 0 && mapX < mapWidth && mapY >= 0 && mapY < mapHeight)
                {
                    char symbol = GetAsteroidSymbol(asteroid.ResourceType);
                    map[mapY, mapX] = symbol;
                }
            }
            
            // Plot ship
            int shipX = (int)((ship.X / worldSize) * mapWidth);
            int shipY = (int)((ship.Y / worldSize) * mapHeight);
            if (shipX >= 0 && shipX < mapWidth && shipY >= 0 && shipY < mapHeight)
            {
                map[shipY, shipX] = 'S';
            }

            Console.WriteLine("\nMini Map:");
            for (int y = 0; y < mapHeight; y++)
            {
                string line = "";
                for (int x = 0; x < mapWidth; x++)
                {
                    line += map[y, x];
                }
                Console.WriteLine("|" + line + "|");
            }
        }

        private char GetAsteroidSymbol(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Iron:
                    return 'i';
                case ResourceType.Gold:
                    return 'g';
                case ResourceType.Platinum:
                    return 'p';
                case ResourceType.Quantum:
                    return 'Q';
                default:
                    return '?';
            }
        }
    }
}