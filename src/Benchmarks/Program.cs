using System;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ResponseBenchmarks>();
        }
    }
}
