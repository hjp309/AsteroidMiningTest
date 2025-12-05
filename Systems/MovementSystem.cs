using System;
using System.Collections.Generic;
using System.Linq;
using AsteroidsMining.Entities;

namespace AsteroidsMining.Systems
{
    public class MovementSystem
    {
        private Ship ship;
        private List<Asteroid> asteroids;

        public MovementSystem(Ship ship, List<Asteroid> asteroids)
        {
            this.ship = ship;
            this.asteroids = asteroids;
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
                dx = dx / distance * ship.Speed;
                dy = dy / distance * ship.Speed;
                ship.MoveTo(ship.X + dx, ship.Y + dy);
            }
            else
            {
                ship.MoveTo(targetX, targetY);
            }
        }

        private Asteroid FindNearestUnminedAsteroid()
        {
            return asteroids
                .Where(a => !a.IsDepleted)
                .OrderBy(a => CalculateDistance(ship.X, ship.Y, a.X, a.Y))
                .FirstOrDefault();
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