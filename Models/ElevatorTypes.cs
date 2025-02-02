using System;
using System.Collections.Generic;

namespace ElevatorManager.Models
{
    public static class ElevatorTypes
    {
        // Static collection of elevator types
        public static readonly List<ElevatorType> Types = new List<ElevatorType>
            {
                new ElevatorType(1, 750)
                //,new ElevatorType(2, 3000)
                //,new ElevatorType(3, 2000)
                //,new ElevatorType(4, 2500)
            };

        public static ElevatorType GetElevatorType(int elevatorTypeId)
        {
            foreach (var type in Types)
            {
                if (type.ElevatorTypeID == elevatorTypeId)
                {
                    return type;
                }
            }
            throw new ArgumentException($"Elevator type '{elevatorTypeId}' not found.");
        }
    }
}