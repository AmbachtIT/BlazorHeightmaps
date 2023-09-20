using Ambacht.Common.Serialization;

namespace Ambacht.Common.Diagnostics
{
    public class RegressionTestHelper
    {
        public RegressionTestHelper(string path, IJsonSerializer serializer)
        {
            Directory.CreateDirectory(Path.Combine(path, "expected"));
            Directory.CreateDirectory(Path.Combine(path, "actual"));

            _path = path;
            _serializer = serializer;
        }

        private readonly string _path;
        private readonly IJsonSerializer _serializer;


        public async Task CheckIfEqualToExpected(object obj, string filename)
        {
            Clean(obj);
            var serialized = obj as string;
            if (serialized == null)
            {
                serialized = _serializer.SerializeObject(obj);
            }

            var pathActual = Path.Combine(_path, "actual", filename);
            var pathExpected = Path.Combine(_path, "expected", filename);
            await File.WriteAllTextAsync(pathActual, serialized);

            var expected = await File.ReadAllBytesAsync(pathExpected);
            var actual = await File.ReadAllBytesAsync(pathActual);

            if (!actual.SequenceEqual(expected))
            {
                throw new InvalidOperationException($"File contents are different");
            }
        }


        private void Clean(object obj)
        {
            foreach (var visit in new ObjectVisitor().VisitAll(obj))
            {

                if (visit.Type == typeof(double))
                {
                    var value = (double)visit.Value();
                    visit.Set(Math.Round(value, 2));
                }
                else if (visit.Type == typeof(float))
                {
                    var value = (float) visit.Value();
                    visit.Set((float)Math.Round(value, 2));
                }
            }
        }


    }
}
