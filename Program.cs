using ElevatorManager.Components;
using ElevatorManager.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorManager
{
    internal class Program
    {
        private static readonly int numberOfFloors = int.Parse(ConfigurationManager.AppSettings["NumberOfFloors"]); //demonstrating another way of storing a value
        private static readonly int averagePersonWeight = int.Parse(ConfigurationManager.AppSettings["AveragerPersonWeight"]);

        static async Task Main(string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out int numberElevatorsParam) || numberElevatorsParam <= 0)
            {
                Console.WriteLine("Usage: ElevatorManagementApp <number_of_elevators>");
                numberElevatorsParam = 5; //default to 5 elevators if no parameter is sent
            }

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IManager, Manager>()
                .BuildServiceProvider();

            var manager = serviceProvider.GetService<IManager>();

            //Manager manager = Manager.Instance;

            // Initialize the elevators
            await manager.InitializeElevators(numberElevatorsParam, numberOfFloors);

            // Start the status display task
            CancellationTokenSource cts = new CancellationTokenSource();
            var statusTask = DisplayElevatorStatusesAsync(manager, cts.Token);

            //Console.WriteLine("Please enter a floor number and number of waiting passengers separated by a space, or type 'exit' to quit.");

            bool keepListening = true;

            while (keepListening)
            {
                Console.WriteLine("Please enter a floor number and number of waiting passengers separated by a space, or type 'exit' to quit.");
                Console.Write("\n> "); // Prompt for input
                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Please enter a command.");
                    continue;
                }

                if (input.ToLower() == "exit")
                {
                    keepListening = false;
                    continue;
                }

                if (TryParseInput(input, out int floorNumber, out int numberOfPersons))
                {
                    Console.WriteLine($"Floor Number: {floorNumber}, Number of Persons: {numberOfPersons}");

                    if (floorNumber <= numberOfFloors)
                    {
                        Console.WriteLine($"You selected floor {floorNumber}. Searching for most suitable elevator...");
                        int requestedLoad = numberOfPersons * averagePersonWeight;
                         while (requestedLoad > 0)
                        {
                            int closestElevatorId = manager.GetClosestElevatorToFloor(floorNumber, requestedLoad, out int handledLoad);
                            if (closestElevatorId > 0)
                            {
                                await manager.CallElevatorAsync(closestElevatorId, floorNumber, handledLoad); // Fixed line
                                                                                                                // Add actions for each valid input if needed
                            }
                            requestedLoad -= handledLoad;
                        }

                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid input. Please enter a number within the valid range.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please provide two integers separated by a space.");
                }
            }
        }

        static bool TryParseInput(string input, out int floorNumber, out int numberOfPersons)
        {
            floorNumber = 0;
            numberOfPersons = 0;

            // Split the input into parts
            string[] parts = input.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);

            // Ensure there are exactly two parts
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out floorNumber) &&
                int.TryParse(parts[1], out numberOfPersons))
            {
                return true;
            }

            return false;
        }
        static async Task DisplayElevatorStatusesAsync(IManager manager, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                //Console.Clear();
                Console.WriteLine("Elevator Statuses:");
                foreach (var elevator in manager.Elevators)
                {
                    Console.WriteLine($"Elevator {elevator.Id}: " +
                                      $"Current Floor = {elevator.CurrentFloor}, " +
                                      $"Target Floor = {elevator.TargetFloor}, " +
                                      $"Current Load = {elevator.CurrentLoad} kg");
                }
                Console.WriteLine("Press <ENTER> to interact with an elevator");
                await Task.Delay(20000, cancellationToken); // Wait for 3 seconds before updating
            }
        }
    }
    
}
