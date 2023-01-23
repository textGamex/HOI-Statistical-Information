using HOI_Message.Logic;
using Throws = NUnit.Framework.Throws;

namespace HOI_Message_UnitTest.Logic
{
    [TestFixture]
    public class DescriptorTests
    {
        private const string TestContent = """
            version="1.0"
            tags={
            	"Gameplay"
            	"Balance"
            	"Alternative History"
            	"Historical"
            	"Map"
            	"Military"
            }
            dependencies={
            	"Old World Blues"
            	"Hearts of Iron IV: The Great War"
            	"North America Divided"
            	"The Road to 56"
            	"Kaiserreich"
            }
            replace_path="history/units"
            name="Test Name"
            picture="thumbnail.png"
            supported_version="1.12.*"
            remote_file_id="123123"
            """;

        private readonly Descriptor _descriptor = new("descriptor.mod", TestContent);

        [Test]
        public void TestProperty()
        {
            var tags = new []{ "Gameplay", "Balance", "Alternative History", "Historical", "Map", "Military" };

            Multiple(() =>
            {
                That(_descriptor.Name, Is.EqualTo("Test Name"));
                That(_descriptor.SupportedVersion, Is.EqualTo("1.12.*"));
                That(_descriptor.PictureName, Is.EqualTo("thumbnail.png"));
                That(_descriptor.Version, Is.EqualTo("1.0"));
                That(_descriptor.Tags, Is.EquivalentTo(tags));
            });
        }

        [Test]
        public void TestExceptionThrow()
        {
            Multiple(() =>
            {
                That(() => { new Descriptor("ErrorPath"); }, Throws.TypeOf<FileNotFoundException>());
            });
        }
    }
}
