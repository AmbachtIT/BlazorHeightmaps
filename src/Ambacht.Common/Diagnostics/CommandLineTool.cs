using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Diagnostics
{
    public class CommandLineTool
    {

        public CommandLineTool(string toolPath)
        {
            this.toolPath = toolPath;
        }

        private readonly StringBuilder arguments = new StringBuilder();
        private readonly Dictionary<string, string> _environmentVariables = new();
        private readonly string toolPath;

        public async Task<int> Run()
        {
            var args = arguments.ToString();
            var startInfo = new ProcessStartInfo(toolPath, args)
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            var info = new FileInfo(toolPath);
            if (info.Exists && info.DirectoryName != null)
            {
                startInfo.WorkingDirectory = info.DirectoryName;
            }
            foreach (var kv in _environmentVariables)
            {
                startInfo.EnvironmentVariables.Add(kv.Key, kv.Value);
            }
            if (LogInvocation)
            {
                await StandardOutput.WriteLineAsync($"{toolPath} {args}");
            }

            using (var process = new Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            })
            {

                var tcsResult = new TaskCompletionSource<int>();
                var tcsStandardOutput = new TaskCompletionSource();
                var tcsStandardError = new TaskCompletionSource();

                process.OutputDataReceived += async (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        await StandardOutput.WriteLineAsync(args.Data);
                    }
                    else
                    {
                        tcsStandardOutput.SetResult();
                    }
                };

                process.ErrorDataReceived += async (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        await StandardError.WriteLineAsync(args.Data);
                    }
                    else
                    {
                        tcsStandardError.SetResult();
                    }
                };

                process.Exited += async (sender, args) =>
                {
                    await tcsStandardOutput.Task.ConfigureAwait(false);
                    await tcsStandardError.Task.ConfigureAwait(false);

                    tcsResult.SetResult(process.ExitCode);
                };

                if (!process.Start())
                {
                    throw new InvalidOperationException("Unable to start process");
                }
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                return await tcsResult.Task.ConfigureAwait(false);
            }

        }

        public CommandLineTool AddArgument(string argument)
        {
            if (arguments.Length > 0)
            {
                arguments.Append(" ");
            }
            arguments.Append(argument);
            return this;
        }

        public CommandLineTool AddEnvironmentVariable(string key, string value)
        {
            _environmentVariables[key] = value;
            return this;
        }

        public TextWriter StandardOutput { get; set; } = Console.Out;

        public TextWriter StandardError { get; set; } = Console.Error;

        public TextReader StandardIn { get; set; } = Console.In;
        public bool LogInvocation { get; set; }
    }
}
