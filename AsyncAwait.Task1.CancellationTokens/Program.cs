/*
* Study the code of this application to calculate the sum of integers from 0 to N, and then
* change the application code so that the following requirements are met:
* 1. The calculation must be performed asynchronously.
* 2. N is set by the user from the console. The user has the right to make a new boundary in the calculation process,
* which should lead to the restart of the calculation.
* 3. When restarting the calculation, the application should continue working without any failures.
*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens;

internal class Program
{
    private static bool isToBeCancelled = false;
    private static bool isCancelled = false;
    /// <summary>
    /// The Main method should not be changed at all.   
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        Console.WriteLine("Mentoring program L2. Async/await.V1. Task 1");
        Console.WriteLine("Calculating the sum of integers from 0 to N.");
        Console.WriteLine("Use 'q' key to exit...");
        Console.WriteLine();

        Console.WriteLine("Enter N: ");

        var input = Console.ReadLine();
        while (input.Trim().ToUpper() != "Q")
        {
            if (int.TryParse(input, out var n))
            {
                CalculateSum(n);
            }
            else
            {
                Console.WriteLine($"Invalid integer: '{input}'. Please try again.");
                Console.WriteLine("Enter N: ");
            }

            input = Console.ReadLine();
            if(!isCancelled)
            {
                isToBeCancelled = true;
            }
            else
                isCancelled = false;
        }

        Console.WriteLine("Press any key to continue");
        Console.ReadLine();
    }

    private static async Task CalculateSum(int n)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        var token = cts.Token;

        Task<long> sum = Calculator.CalculateAsync(n, token);
        var cancelTask = Task.Run(async () =>
        {
            Console.WriteLine($"The task for {n} started... Enter N to cancel the request:");
            await Task.Delay(1000);

            if (isToBeCancelled)
            {
                isToBeCancelled = false;
                isCancelled = true;
                cts.Cancel();
                cts.Dispose();
                cts = new CancellationTokenSource();
            }
        });

        await Task.WhenAny(sum, cancelTask);
        if (token.IsCancellationRequested)
        {
            Console.WriteLine($"Sum for {n} cancelled...");
        }
        else
        {
            Console.WriteLine($"Sum for {n} = {await sum}.");
            Console.WriteLine();
        }
    }
}
