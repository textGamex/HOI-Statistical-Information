using HOI_Message.Logic.Util.CWTool;

namespace HOI_Message_UnitTest.Logic.CWTools
{
    [TestFixture]
    public class CWToolsAdapterTests
    {
        private const string TestText = """
            key1 = testValue
            key2 = { value1 value2 1 2 }
            key3 = {
                key3-1 = value
            }
            """;
        private readonly CWToolsAdapter _adapter = new("test.txt", TestText);

        [Test]
        public void KeyTest()
        {
            var result = _adapter.Root;
            
            Multiple(() =>
            {
                That(_adapter.IsSuccess, Is.True);
                That(result.Has("key1"), Is.True);
                That(result.Has("key2"), Is.True);
                That(result.Has("key3"), Is.True);
            });
        }

        [Test]
        public void ValueTest()
        {
            var result = _adapter.Root;
            var key1Value = result.Leafs("key1");
            var key2Value = result.Leafs("key2");
            var key3Value = result.Leafs("key3");

            Multiple(() =>
            {
                That(key1Value.Count(), Is.EqualTo(1));
                That(key2Value.Count(), Is.EqualTo(0));
                That(key3Value.Count(), Is.EqualTo(0));
            });
        }
    }
}
