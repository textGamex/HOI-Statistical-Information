using HOI_Message.Logic.Localisation;
using Throws = NUnit.Framework.Throws;

namespace HOI_Message_UnitTest.Logic.Localisation
{
    [TestFixture]
    public class GameLocalisationTests
    {
        [Test]
        public void TestAddByFilePathExceptionThrow()
        {
            var gameLocalisation = new GameLocalisation();

            Multiple(() =>
            {
                That(() => { gameLocalisation.AddByFilePath("ErrorPath"); }, Throws.TypeOf<FileNotFoundException>());
            });
        }

        [Test]
        public void AddByMap_StateUnderTest_ExpectedBehavior()
        {
            var gameLocalisation = new GameLocalisation();
            var map = new Dictionary<string, LocalisationParser.LineData>
            {
                { "key1", new LocalisationParser.LineData("key1", "value1") },
                { "key2", new LocalisationParser.LineData("key2", "value2", 1) },
                { "key3", new LocalisationParser.LineData("key3", "value3-1", 1) }
            };
            var eepetitive = new Dictionary<string, LocalisationParser.LineData>
            {
                {"key3", new LocalisationParser.LineData("key3", "value3-2", 2)}
            };

            gameLocalisation.AddByMap(map);
            gameLocalisation.AddByMap(eepetitive);

            Multiple(() =>
            {
                That(gameLocalisation.GetValue("key1"), Is.EqualTo("value1"));
                That(gameLocalisation.GetValue("key2"), Is.EqualTo("value2"));
                That(gameLocalisation.GetValue("key3"), Is.EqualTo("value3-2"));
                That(gameLocalisation.GetValue("key4"), Is.EqualTo("key4"));
            });
        }
    }
}
