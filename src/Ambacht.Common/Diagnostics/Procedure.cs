using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Diagnostics
{
    public abstract class Procedure
    {

        public TextWriter Output { get; set; } = Console.Out;

        public bool LogDuration { get; set; }

        public async Task Run()
        {
            await Run(new Progress(CancellationToken.None));
        }

        public async Task Run(Progress progress)
        {
            if (LogDuration)
            {
                await Output.WriteLineAsync($"Procedure {Name}: Starting");
            }
            var start = DateTime.UtcNow;
            await RunCore(progress);
            var duration = DateTime.UtcNow - start;
            if (LogDuration)
            {
                await Output.WriteLineAsync($"Procedure {Name}: Took {duration}");
            }
        }

        public abstract Task RunCore(Progress progress);

        public virtual string Name => GetType().Name;

    }

    
}
