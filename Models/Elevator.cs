using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorManager.Models
{
    public class Elevator
    {
        public int Id { get; set; }
        public int CurrentFloor { get; set; }
        public int TargetFloor { get; set; }
        public int CurrentLoad { get; set; }
        public int ElevatorTypeId { get; set; }
        public int MaxLoad
        {
            get
            {
                return ElevatorTypes.GetElevatorType(ElevatorTypeId).MaxLoad;
            }
        }

        public override string ToString()
        {
            return $"Elevator {Id}: Floor {CurrentFloor}, MaxLoad {ElevatorTypes.GetElevatorType(ElevatorTypeId).MaxLoad}kg";
        }
    }
}
