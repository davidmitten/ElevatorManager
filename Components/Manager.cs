using ElevatorManager.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorManager.Components
{
    public class Manager : IManager
    {
        private static readonly int doorOperationTime = int.Parse(ConfigurationManager.AppSettings["DoorOperationTime"]); // Time for door operations
        private static readonly int personOperationTime = int.Parse(ConfigurationManager.AppSettings["PersonOperationTime"]); // Time for person operations

        // List of elevators
        public List<Elevator> Elevators { get; private set; }

        public Manager()
        {
            Elevators = new List<Elevator>();
        }

        // Initialize the elevators
        public async Task InitializeElevators(int numberOfElevators, int numberOfFloors)
        {
            for (int i = 1; i <= numberOfElevators; i++)
            {
                Elevators.Add(new Elevator
                {
                    Id = i,
                    CurrentFloor = 0,
                    TargetFloor = 0,
                    CurrentLoad = 0,
                    ElevatorTypeId = 1   //MaxLoad is derived from the Type
                });
            }
        }

        // Get the current status of all elevators
        public List<Elevator> GetAllElevators()
        {
            return Elevators.ToList(); // Return a copy to avoid direct modification
        }

        // Update an elevator's status
        public async Task CallElevatorAsync(int id, int floor, int load)
        {
            int numberOfFloors = int.Parse(ConfigurationManager.AppSettings["NumberOfFloors"]); //demonstrating another way of storing a value

            if (floor > numberOfFloors)
            {
                Console.WriteLine($"There are only {numberOfFloors} in this building");
                return;
            }
            var elevator = Elevators.FirstOrDefault(e => e.Id == id);
            if (elevator == null)
            {
                Console.WriteLine($"Elevator {id} not found!");
                return;
            }

            elevator.TargetFloor = floor;
            // Perform a wait period for door and person operations
            int waitTime = (doorOperationTime * 2) + (personOperationTime * 2);
            await Task.Delay(waitTime);

            elevator.CurrentFloor = floor;
            if (elevator.CurrentLoad + load <= elevator.MaxLoad)
            {
                elevator.CurrentLoad += load;
            }

            Console.WriteLine($"Elevator {id} updated successfully.");
        }

        public int GetClosestElevatorToFloor(int requestedFloor, int requestedLoad, out int HandledLoad)
        {
            // Find the suitable elevators that can handle the requested load
            var suitableElevators = Elevators
     .Where(e => (e.CurrentLoad < e.MaxLoad) && (e.CurrentLoad + requestedLoad >= 0)) // Filter elevators that can handle the requested load
     .OrderBy(e => Math.Abs(e.CurrentFloor - requestedFloor)) // Sort by proximity to the requested floor
     .ThenByDescending(e => e.MaxLoad - e.CurrentLoad); // Then sort by maximum available load capacity

            var closestElevator = suitableElevators.FirstOrDefault();

            if (closestElevator != null)
            {
                HandledLoad = Math.Min(closestElevator.MaxLoad - closestElevator.CurrentLoad, requestedLoad);
                return closestElevator.Id; // Return the ID of the closest suitable elevator
            }
            else
            {
                HandledLoad = 0;
                return 0;
            }

            //else
            //{
            //    // If no elevator can handle the full load, handle the load in chunks
            //    var elevatorWithMaxLoadCapacity = Elevators
            //        .OrderByDescending(e => e.MaxLoad)
            //        .FirstOrDefault();

            //    if (elevatorWithMaxLoadCapacity != null)
            //    {
            //        int maxLoad = elevatorWithMaxLoadCapacity.MaxLoad;
            //        if (requestedLoad > maxLoad)
            //        {
            //            // Call the elevator with the maximum load capacity
            //            CallElevatorAsync(elevatorWithMaxLoadCapacity.Id, requestedFloor, maxLoad).Wait();

            //            // Recursively call the method with the remaining load
            //            return GetClosestElevatorToFloor(requestedFloor, requestedLoad - maxLoad);
            //        }
            //    }

            //    Console.WriteLine($"No elevators can accommodate the requested load {requestedLoad}kg at this time.");
            //    return 0;
            //}
        }

        public int GetClosestElevatorToFloorold(int requestedFloor, int requestedLoad)
        {
            var suitableElevators = Elevators
                .Where(e => (e.CurrentLoad + requestedLoad <= e.MaxLoad) && (e.CurrentLoad + requestedLoad >= 0)) // Filter elevators that can handle the requested load
                .OrderBy(e => Math.Abs(e.CurrentFloor - requestedFloor)); // Sort by proximity to the requested floor

            var closestElevator = suitableElevators.FirstOrDefault();

            if (closestElevator != null)
            {
                return closestElevator.Id; // Return the ID of the closest suitable elevator
            }
            else
            {
                Console.WriteLine($"No elevators can accommodate the requested load {requestedLoad}kg at this time.");
                return 0;
            }
        }
    }
}
