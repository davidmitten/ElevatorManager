using ElevatorManager.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ElevatorManager.Components
{
    public interface IManager
    {
        Task InitializeElevators(int numberOfElevators, int numberOfFloors);
        List<Elevator> Elevators { get; }

        Task CallElevatorAsync(int id, int floor, int load);
        List<Elevator> GetAllElevators();
        int GetClosestElevatorToFloor(int requestedFloor, int requestedLoad, out int HandledLoad);
    }
}