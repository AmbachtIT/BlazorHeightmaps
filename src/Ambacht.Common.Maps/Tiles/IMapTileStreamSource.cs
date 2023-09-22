using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.IO;

namespace Ambacht.Common.Maps.Tiles
{
	public interface IMapTileStreamSource
	{

		Task<Stream> GetStream(IMapTile tile);

	}


	public class HttpMapTileStreamSource : IMapTileStreamSource
	{
		public HttpMapTileStreamSource(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		private readonly HttpClient _httpClient;


		public async Task<Stream> GetStream(IMapTile tile)
		{
			return await _httpClient.GetStreamAsync(tile.Url);
		}
	}

	public class CachingTileStreamSource : IMapTileStreamSource
	{
		public CachingTileStreamSource(IMapTileStreamSource source, IFileSystem store)
		{
			_source = source;
			_store = store;
		}

		private readonly IMapTileStreamSource _source;
		private readonly IFileSystem _store;

		public async Task<Stream> GetStream(IMapTile tile)
		{
			var path = tile.Key;
			if (!(await _store.Exists(path)))
			{
				using (var source = await _source.GetStream(tile))
				using(var target = await _store.OpenWrite(path))
				{
					await source.CopyToAsync(target);
				}
			}

			return await _store.OpenRead(path);
		}

	}

}
