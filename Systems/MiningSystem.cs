using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsteroidsMining.Entities;

namespace AsteroidsMining.Systems
{
    public class MiningSystem
    {
        private Ship ship;
        private AsteroidPool asteroidPool;

        public static event Action<string> OnMiningComplete;

        public MiningSystem(Ship ship, AsteroidPool asteroidPool)
        {
            this.ship = ship;
            this.asteroidPool = asteroidPool;
        }

        public async Task ProcessMining()
        {
            // Fix: Removed nested for loops and consolidated conditional logic.
            var nearbyAsteroids = new List<Asteroid>();
            foreach (var asteroid in asteroidPool.asteroids)
            {
                if (asteroid.GetDistanceTo(ship.X, ship.Y) < 10.0 && ship.CanMine(asteroid)){
                    nearbyAsteroids.Add(asteroid);
                }
            }

            // Mine the first available asteroid
            if (nearbyAsteroids.Count > 0)
            {
                var asteroidToMine = nearbyAsteroids[0];
                
                await Task.Delay((int)(asteroidToMine.MiningDifficulty * 1000));
                
                var resource = asteroidToMine.Mine();
                if (resource != null)
                {
                    bool added = ship.AddResource(resource);
                    if (added)
                    {
                        OnMiningComplete?.Invoke($"Mined {resource} at ({asteroidToMine.X}, {asteroidToMine.Y})"); // FIX: Redundant messaging and unnecessary subscription.
                    }
                }
            }
        }

        public Asteroid FindNearestAsteroid()
        {
            Asteroid nearest = null;
            double minDistance = double.MaxValue;

            foreach (var asteroid in asteroidPool.asteroids)
            {
                double distance = asteroid.GetDistanceTo(ship.X, ship.Y);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = asteroid;
                }
            }

            return nearest;
        }
    }
}