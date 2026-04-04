using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Benchmark.Editor
{
	public static class BenchmarkBuild
	{
		private const string DefaultExecutableName = "Ecs.Benchmark.Unity";
		private const string MenuRoot = "Benchmark/Build/";
		private const string BuildTargetArgument = "-benchmarkBuildTarget";

		[MenuItem(MenuRoot + "Active Target")]
		public static void BuildActiveTarget() =>
			Build(EditorUserBuildSettings.activeBuildTarget);

		public static void BuildBatchMode() =>
			Build(ResolveTarget(Environment.GetCommandLineArgs()));

		private static void Build(BuildTarget target)
		{
			var scenes = EditorBuildSettings.scenes
											.Where(scene => scene.enabled)
											.Select(scene => scene.path)
											.ToArray();
			if (scenes.Length == 0)
				throw new InvalidOperationException("No enabled scenes in EditorBuildSettings.");

			var outputPath = ResolveOutputPath(target);
			var outputDirectory = Path.GetDirectoryName(outputPath);
			if (string.IsNullOrEmpty(outputDirectory))
				throw new InvalidOperationException("Could not resolve output directory.");
			Directory.CreateDirectory(outputDirectory);

			var options = new BuildPlayerOptions
			{
				scenes = scenes,
				locationPathName = outputPath,
				target = target,
				options = BuildOptions.None,
			};

			Debug.Log($"Building {target} to {outputPath}");
			var report = BuildPipeline.BuildPlayer(options);
			if (report.summary.result != BuildResult.Succeeded)
				throw new InvalidOperationException(
					$"Build failed: {report.summary.result}. " +
					$"Errors: {report.summary.totalErrors}, Warnings: {report.summary.totalWarnings}");

			Debug.Log(
				$"Build succeeded: {outputPath}\n" +
				$"Size: {report.summary.totalSize} bytes\n" +
				$"Duration: {report.summary.totalTime}");
		}

		private static string ResolveOutputPath(BuildTarget target)
		{
			var commandLine = Environment.GetCommandLineArgs();
			var customPath = TryGetArgument(commandLine, "-benchmarkBuildPath");
			if (!string.IsNullOrWhiteSpace(customPath))
				return Path.GetFullPath(customPath);

			var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
			var outputDirectory = Path.Combine(projectRoot, "Build", target.ToString());
			return target switch
			{
				BuildTarget.StandaloneLinux64 => Path.Combine(outputDirectory, DefaultExecutableName),
				BuildTarget.StandaloneWindows64 => Path.Combine(outputDirectory, $"{DefaultExecutableName}.exe"),
				BuildTarget.StandaloneOSX => Path.Combine(outputDirectory, $"{DefaultExecutableName}.app"),
				BuildTarget.Android => Path.Combine(outputDirectory, $"{DefaultExecutableName}.apk"),
				BuildTarget.iOS => outputDirectory,
				BuildTarget.WebGL => outputDirectory,
				_ => throw new InvalidOperationException($"Unsupported target: {target}"),
			};
		}

		private static BuildTarget ResolveTarget(string[] args)
		{
			var targetArgument = TryGetArgument(args, BuildTargetArgument);
			if (!string.IsNullOrWhiteSpace(targetArgument))
			{
				if (Enum.TryParse<BuildTarget>(targetArgument, true, out var parsed))
					return parsed;

				throw new InvalidOperationException($"Invalid build target: {targetArgument}");
			}

			return EditorUserBuildSettings.activeBuildTarget;
		}

		private static string TryGetArgument(string[] args, string name)
		{
			for (var i = 0; i < args.Length - 1; i++)
			{
				if (!string.Equals(args[i], name, StringComparison.Ordinal))
					continue;
				return args[i + 1];
			}

			return null;
		}
	}
}
