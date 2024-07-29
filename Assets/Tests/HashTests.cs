using System.Linq;
using NUnit.Framework;

namespace Benchmark.Tests
{

public class HashTests : HashTestsBase
{
	[TestCase(1,    1)]
	[TestCase(2,    4)]
	[TestCase(4,    16)]
	[TestCase(8,    64)]
	[TestCase(256,  16)]
	[TestCase(2048, 32)]
	public void Check(int entityCount, int ticks) =>
		TestContexts(Contexts.Factories.Select(factory => factory.factory()), entityCount, ticks);

	[TestCase(16, 256)]
	[TestCase(32, 1024)]
	public void CheckLong(int entityCount, int ticks) =>
		Check(entityCount, ticks);
}

}
