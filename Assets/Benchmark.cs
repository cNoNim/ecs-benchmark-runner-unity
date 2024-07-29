using System.Collections.Generic;
using System.Linq;
using Benchmark.Core;
using Benchmark.Unity;
using Benchmark_Contexts = Benchmark.Contexts;

namespace Benchmark.Runner
{

public class Benchmark : BenchmarkBase
{
	protected override IEnumerable<IContext> Contexts
	{
		get => Benchmark_Contexts.Factories.Select(factory => factory.factory());
	}
}

}
