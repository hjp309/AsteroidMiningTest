using System;
using System.Collections.Generic;
using System.Linq;
using AsteroidsMining.Entities;

namespace AsteroidsMining.Systems
{
    public class MovementSystem
    {
        private Ship ship;
        private AsteroidPool asteroidPool;

        public MovementSystem(Ship ship, AsteroidPool asteroidPool)
        {
            this.ship = ship;
            this.asteroidPool = asteroidPool;
        }

        public void MoveToNearestAsteroid()
        {
            var target = FindNearestUnminedAsteroid();
            if (target != null)
            {
                MoveShipTowards(target.X, target.Y);
            }
        }

        public void ReturnToBase()
        {
            // Base is at (0, 0)
            MoveShipTowards(0, 0);
        }

        private void MoveShipTowards(double targetX, double targetY)
        {
            double dx = targetX - ship.X;
            double dy = targetY - ship.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance > ship.Speed)
            {
                dx = dx * ship.Speed / distance;    // Get scale dx and dy by ratio of ship.Speed to distance  
                dy = dy * ship.Speed / distance;

                ship.MoveTo(ship.X + dx, ship.Y + dy);
            }
            else
            {
                ship.MoveTo(targetX, targetY);
            }
        }

        // FIX: More efficient. Not looping through the entire list. O(N). AI Assisted.
        private Asteroid FindNearestUnminedAsteroid()
        {
            Asteroid nearest = null;
            double bestDist = double.MaxValue;

            foreach (var a in asteroidPool.asteroids)
            {
                double dx = a.X - ship.X;
                double dy = a.Y - ship.Y;
                double dist = dx * dx + dy * dy;

                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = a;
                }
            }

            return nearest;
        }

        private double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public List<(double X, double Y)> CalculatePath(double targetX, double targetY)
        {
            var path = new List<(double, double)>();

            double steps = CalculateDistance(ship.X, ship.Y, targetX, targetY) / ship.Speed;
            for (int i = 0; i < steps * 10; i++)
            {
                double progress = i / (steps * 10);
                double x = ship.X + (targetX - ship.X) * progress;
                double y = ship.Y + (targetY - ship.Y) * progress;
                path.Add((x, y));
            }
            
            return path;
        }
    }
}