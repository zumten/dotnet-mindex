using System.IO;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZumtenSoft.Mindex.Stubs;
using RunMode = BenchmarkDotNet.Jobs.RunMode;

namespace ZumtenSoft.Mindex.Benchmark.IndianCustoms
{
    public class CustomConfig : ManualConfig
    {
        public CustomConfig()
        {
            var runMode = RunMode.Default;

            ArtifactsPath = Path.Combine(FileHelper.FindDirectory("ZumtenSoft.Mindex.Benchmark"), "BenchmarkDotNet.Artifacts");
            //Add(new Job(EnvMode.Clr, RunMode.Dry));
            Add(new Job(".NET Core 2 (x64)", EnvMode.Core, EnvMode.RyuJitX64, runMode),
                new Job(".NET Framework 4.7 (x64)", EnvMode.Clr, EnvMode.RyuJitX64, runMode),
                new Job(".NET Framework 4.7 (x86)", EnvMode.Clr, EnvMode.RyuJitX86, runMode));
            Add(MemoryDiagnoser.Default);
            Add(CsvExporter.Default, RPlotExporter.Default, MarkdownExporter.Default, HtmlExporter.Default);
            Add(ConsoleLogger.Default);
            Add(EnvironmentAnalyser.Default, OutliersAnalyser.Default, MinIterationTimeAnalyser.Default, IterationSetupCleanupAnalyser.Default, MultimodalDistributionAnalyzer.Default);
            Add(BaselineValidator.FailOnError, SetupCleanupValidator.FailOnError, JitOptimizationsValidator.FailOnError, UnrollFactorValidator.Default);
            Add(DefaultColumnProviders.Target,
                DefaultColumnProviders.Job,
                DefaultColumnProviders.Statistics,
                DefaultColumnProviders.Params,
                DefaultColumnProviders.Diagnosers);
        }
    }

#if !DEBUG
    [TestClass]
#endif
    public class IndianCustomsImportBenchmarkTests
    {
        [TestMethod]
        public void SearchByOriginDestinationQuantityTypeDateBenchmark()
        {
            BenchmarkRunner.Run<SearchByOriginDestinationQuantityTypeDate>(new CustomConfig());
        }

        [TestMethod]
        public void SearchBySingleOriginBenchmark()
        {
            BenchmarkRunner.Run<SearchBySingleOrigin>(new CustomConfig());
        }

        [TestMethod]
        public void SearchByMultipleOriginsBenchmark()
        {
            BenchmarkRunner.Run<SearchByMultipleOrigins>(new CustomConfig());
        }
    }
}
