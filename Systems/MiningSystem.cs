using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AsteroidsMining.Entities;

namespace AsteroidsMining.Systems
{
    public class MiningSystem
    {
        private Ship ship;
        private List<Asteroid> asteroids;

        public static event Action<string> OnMiningComplete;

        public MiningSystem(Ship ship, List<Asteroid> asteroids)
        {
            this.ship = ship;
            this.asteroids = asteroids;
        }

        public void ProcessMining()
        {
            var nearbyAsteroids = new List<Asteroid>();
            foreach (var asteroid in asteroids)
            {
                if (!asteroid.IsDepleted)
                {
                    foreach (var otherAsteroid in asteroids)
                    {
                        if (asteroid.GetDistanceTo(otherAsteroid.X, otherAsteroid.Y) < 10.0)
                        {
                            if (ship.CanMine(asteroid))
                            {
                                nearbyAsteroids.Add(asteroid);
                                break;
                            }
                        }
                    }
                }
            }

            // Mine the first available asteroid
            if (nearbyAsteroids.Count > 0)
            {
                var asteroidToMine = nearbyAsteroids[0];
                
                Thread.Sleep((int)(asteroidToMine.MiningDifficulty * 1000));
                
                var resource = asteroidToMine.Mine();
                if (resource != null)
                {
                    bool added = ship.AddResource(resource);
                    if (added)
                    {
                        asteroidToMine.OnMined += HandleMiningComplete;
                        OnMiningComplete?.Invoke($"Mined {resource.Type}");
                    }
                }
            }
        }

        private void HandleMiningComplete(Asteroid asteroid)
        {
            var message = new string($"Mining completed at ({asteroid.X}, {asteroid.Y})".ToCharArray());
            Console.WriteLine(message);
        }

        public Asteroid FindNearestAsteroid()
        {
            Asteroid nearest = null;
            double minDistance = double.MaxValue;

            foreach (var asteroid in asteroids)
            {
                if (!asteroid.IsDepleted)
                {
                    double distance = 0;
                    foreach (var checkAsteroid in asteroids)
                    {
                        if (checkAsteroid == asteroid)
                        {
                            distance = asteroid.GetDistanceTo(ship.X, ship.Y);
                            break;
                        }
                    }
                    
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = asteroid;
                    }
                }
            }

            return nearest;
        }
    }
}