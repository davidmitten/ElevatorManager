using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorManager.Models
{
    public class ElevatorType
    {
        public int ElevatorTypeID { get; set; }
        public int MaxLoad { get; set; }
        public int TimeInSecondsForDoorOperation { get; set; } = 5;

        public ElevatorType(int elevatorTypeId, int maxLoad)
        {
            ElevatorTypeID = elevatorTypeId;
            MaxLoad = maxLoad;
        }
    }
    
}
