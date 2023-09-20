using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Diagnostics;

namespace Ambacht.Common.Docker
{
    public class DockerContainer
    {

        public DockerContainer(string image, string name)
        {
            _name = name;
            _image = image;
        }

        private readonly string _image;
        private readonly string _name;
        private readonly Dictionary<string, string> _environmentVariables = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _mappedPorts = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _mappedVolumes = new Dictionary<string, string>();


        /// <summary>
        /// Is this container started in the detached mode (docker run -d). Default = false
        /// </summary>
        public bool Detached { get; set; }


        /// <summary>
        /// Is this container cleaned up after it exits (docker run -rm). Default = false
        /// </summary>
        public bool CleanUp { get; set; }

        public TextWriter Output { get; set; } = Console.Out;

        public DockerContainer SetEnvironmentVariable(string name, string value)
        {
            _environmentVariables[name] = value;
            return this;
        }

        public DockerContainer MapPort(string from, string to)
        {
            _mappedPorts[from] = to;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">local directory on host computer</param>
        /// <param name="to">directory inside docker container</param>
        /// <returns></returns>
        public DockerContainer MapVolume(string from, string to)
        {
            _mappedVolumes[from] = to;
            return this;
        }

        public async Task Run()
        {
            var tool =
                new CommandLineTool("docker")
                    {
                        LogInvocation = true,
                        StandardOutput = Output
                    }
                    .AddArgument("run")
                    .AddArgument($"--name {_name}");
            if (Detached)
            {
                tool.AddArgument("-d");
            }

            if (CleanUp)
            {
                tool.AddArgument("--rm");
            }

            foreach (var kv in _mappedVolumes)
            {
                tool.AddArgument($"-v {kv.Key}:{kv.Value}");
            }

            foreach (var kv in _environmentVariables)
            {
                tool.AddArgument($"-e {kv.Key}={kv.Value}");
                tool.AddEnvironmentVariable(kv.Key, kv.Value);
            }

            foreach (var kv in _mappedPorts)
            {
                tool.AddArgument($"-p {kv.Key}:{kv.Value}");
            }

            tool.AddArgument(_image);
            await tool.Run();
        }


        public async Task Execute(string command, bool interactive = true)
        {
            var tool =
                new CommandLineTool("docker")
                    {
                        LogInvocation = true,
                        StandardOutput = Output
                    }
                    .AddArgument("exec");
            if (interactive)
            {
                tool.AddArgument("-it");
            }

            tool.AddArgument(_name);
            tool.AddArgument(command);
            foreach (var kv in _environmentVariables)
            {
                tool.AddEnvironmentVariable(kv.Key, kv.Value);
            }

            await tool.Run();
        }

        public async Task Stop()
        {
            await new CommandLineTool("docker")
                {
                    LogInvocation = true,
                    StandardOutput = Output
                }.AddArgument($"stop {_name}")
                .Run();
        }


    }
}
