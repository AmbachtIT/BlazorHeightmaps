using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Tiles;

namespace BlazorHeightmaps.Test.Maps
{
  [TestFixture()]
  public class SlippyMaps
  {


    [Test(), TestCaseSource(nameof(AllSetsAndZoomLevels)), Explicit()]
    public async Task DownloadAll((SlippyTileSet, int) parms)
    {
      var (set, level) = parms;
      var url = set.GetUrl(_utrecht, level);
      Console.WriteLine(url);
      var path = TestHelper.GetSourceRoot("test\\BlazorHeightmaps.Test\\data\\slippy-tiles", $"{set.Name}-{level}.{set.Extension}");
      if (File.Exists(path))
      {
        return;
      }

      using (var client = new HttpClient())
      {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.Clear();
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("AppleWebKit", "537.36"));

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var input = await response.Content.ReadAsStreamAsync();
        using var output = File.Create(path);

        await input.CopyToAsync(output);
      }
    }


    private static readonly LatLng _utrecht = new(52.0907f, 5.1214f);

    private static IEnumerable<(SlippyTileSet, int)> AllSetsAndZoomLevels()
    {
      foreach (var set in SlippyTileSet.All())
      {
        for (var level = set.MinZoom; level <= set.MaxZoom; level++)
        {
          yield return (set, level);
        }
      }
    }

  }
}
