using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Bullseye.Targets;
using static SimpleExec.Command;

namespace build;

sealed partial class Program
{
    private const string PackOutput = "./artifacts";
    private const string EnvVarMissing = " environment variable is missing. Aborting.";

    private static class Targets
    {
        public const string CleanBuildOutput = "clean-build-output";
        public const string CleanPackOutput = "clean-pack-output";
        public const string Build = "build";
        public const string Test = "test";
        public const string Pack = "pack";
    }

    static async Task Main(string[] args)
    {
        Target(Targets.CleanBuildOutput, async () =>
        {
            await RunAsync("dotnet", "clean -c Release -v m --nologo", echoPrefix: Prefix);
        });

        Target(Targets.Build, DependsOn(Targets.CleanBuildOutput), async () =>
        {
            await RunAsync("dotnet", "build -c Release --nologo", echoPrefix: Prefix);
        });

        Target(Targets.Test, DependsOn(Targets.Build), async () =>
        {
            await RunAsync("dotnet", "test -c Release --no-build --nologo", echoPrefix: Prefix);
        });

        Target(Targets.CleanPackOutput, () =>
        {
            if (Directory.Exists(PackOutput))
            {
                Directory.Delete(PackOutput, true);
            }
        });

        Target(Targets.Pack, DependsOn(Targets.Build, Targets.Test, Targets.CleanPackOutput), async () =>
        {
            var project = Directory.GetFiles("./src", "*.csproj", SearchOption.TopDirectoryOnly).First();

            await RunAsync("dotnet", $"pack {project} -c Release -o \"{Directory.CreateDirectory(PackOutput).FullName}\" --no-build --nologo", echoPrefix: Prefix);
        });

        Target("default", DependsOn(Targets.Pack));

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await RunTargetsAndExitAsync(
            args, 
            ex => ex is SimpleExec.ExitCodeException || ex.Message.EndsWith(EnvVarMissing, StringComparison.OrdinalIgnoreCase), 
            getMessagePrefix: () => Prefix);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
    }
}
