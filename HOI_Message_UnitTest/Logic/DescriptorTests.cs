using HOI_Message.Logic;
using Throws = NUnit.Framework.Throws;

namespace HOI_Message_UnitTest.Logic
{
    [TestFixture]
    public class DescriptorTests
    {
        private readonly Descriptor _descriptor = new("Resources\\GameFile\\descriptor.mod");

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
