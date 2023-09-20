using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.IO
{
	public class InMemoryFileSystem : IFileSystem
	{
		public InMemoryFileSystem() : this("Default")
		{
		}

		public InMemoryFileSystem(string name)
		{
			_name = name;
		}

		private readonly string _name;
		private Dictionary<string, byte[]> _files = new();



		public Task<Stream> OpenRead(string path, CancellationToken token = default)
		{
			return Task.FromResult<Stream>(new MemoryStream(_files[path]));
		}

		public Task<Stream> OpenWrite(string path, CancellationToken token = default)
		{
			return Task.FromResult<Stream>(new WritingMemoryStream(this, path));
		}

		public Task<bool> Exists(string path, CancellationToken token = default) => Task.FromResult(_files.ContainsKey(path));

		public override string ToString() => $"InMemory @ {_name}";


		private class WritingMemoryStream : MemoryStream
		{
			public WritingMemoryStream(InMemoryFileSystem system, string path)
			{
				_path = path;
				_system = system;
			}

			private readonly InMemoryFileSystem _system;
			private readonly string _path;
			private bool _saved;

			private void Save()
			{
				if (_saved)
				{
					return;
				}

				_saved = true;
				base.Flush();
				_system._files[_path] = this.ToArray();
			}

			protected override void Dispose(bool disposing)
			{
				Save();
				base.Dispose(disposing);
			}

			public override async ValueTask DisposeAsync()
			{
				Save();
				await base.DisposeAsync();
			}
		}

	}
}
