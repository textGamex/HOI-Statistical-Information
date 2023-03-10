using HOI_Message.Logic;
using HOI_Message.Logic.Unit;
using Throws = NUnit.Framework.Throws;

namespace HOI_Message_UnitTest.Logic.Unit
{
    [TestFixture]
    public class ArmyUnitInfoTests
    {
        [Test]
        public void TestEmptyProperty()
        {
            Multiple(() =>
            {
                That(ArmyUnitInfo.Empty.UnitSum, Is.EqualTo(0));
                That(ArmyUnitInfo.Empty.OwnCountryTag, Is.EqualTo(CountryTag.Empty));
            });
        }

        [Test]
        public void TestProperty()
        {
            var armyUnitInfo = new ArmyUnitInfo(@"Resources\GameFile\LUX_1936.txt", new CountryTag("TES"));

            Multiple(() =>
            {
                That(armyUnitInfo.OwnCountryTag.Tag, Is.EqualTo("TES"));
                That(armyUnitInfo.UnitSum, Is.EqualTo(1));
            });
        }

        [Test]
        public void TestExceptionThrow()
        {
            Multiple(() =>
            {
                That(() => { new ArmyUnitInfo("ErrorPath", new CountryTag("TEX")); }, Throws.TypeOf<FileNotFoundException>());
                That(() => { new ArmyUnitInfo(@"Resources\GameFile\LUX_1936.txt", new CountryTag("TEX")); }, Throws.Nothing);
            });
        }
    }
}
