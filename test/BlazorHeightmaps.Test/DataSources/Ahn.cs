using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps.Wmts;
using Ambacht.OpenData.Sources.Ahn;

namespace BlazorHeightmaps.Test.DataSources
{
	[TestFixture()]
	public class Ahn
	{
		[Test()]
		public void TestGenerateWmtsUrl()
		{
			var url = new WmtsUrlBuilder()
				.Ahn4Ellipsis()
				.Ahn4Dsm5(10000, 10000)
				.Build();

		}

	}
}
