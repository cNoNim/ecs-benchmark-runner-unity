using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Benchmark.Core;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine.TestTools;

namespace Benchmark.Performance.Tests
{

public class Benchmark : BenchmarkBase
{
	private static readonly Dictionary<Type, Func<IContext>> Factories =
		Contexts.Factories.ToDictionary(tuple => tuple.type, tuple => tuple.factory);

	public static IEnumerable Source
	{
		get => Factories.Keys;
	}

	[Performance]
	[UnityTest]
	public IEnumerator Measure([ValueSource(nameof(Source))] Type type) =>
		Factories.TryGetValue(type, out var factory)
			? Routine(
				factory(),
				2048,
				0,
				128,
				1)
			: Array.Empty<object>()
				   .GetEnumerator();
}

}
