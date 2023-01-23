using HOI_Message.Logic.CustomException;
using HOI_Message.Logic.State;
using Throws = NUnit.Framework.Throws;

namespace HOI_Message_UnitTest.Logic.State
{
    [TestFixture]
    public class StateInfoTests
    {
        private readonly StateInfo _stateInfo = new("Resources\\GameFile\\6-Belgium.txt");

        [Test]
        public void PropertyTest()
        {
            Multiple(() =>
            {
                That(_stateInfo.Name, Is.EqualTo("STATE_6"));
                That(_stateInfo.Id, Is.EqualTo(6));
                That(_stateInfo.Manpower, Is.EqualTo(4747700));
                That(_stateInfo.OwnerTag, Is.EqualTo("TEST"));
                That(_stateInfo.Resources, Does.ContainKey("steel").WithValue(25));
                That(_stateInfo.Buildings, Does.ContainKey("infrastructure").WithValue(3));
                That(_stateInfo.Buildings, Does.ContainKey("arms_factory").WithValue(5));
                That(_stateInfo.Buildings, Does.ContainKey("industrial_complex").WithValue(7));
                That(_stateInfo.Buildings, Does.ContainKey("air_base").WithValue(3));
            });
        }

        [Test]
        public void GetHasCoreTagsTest()
        {
            string[] tags = { "TEST" };

            That(_stateInfo.GetHasCoreTags(), Is.EquivalentTo(tags));
        }

        [Test]
        public void ThrowExceptionTest()
        {
            Multiple(() =>
            {
                That(() => { new StateInfo(string.Empty); }, Throws.TypeOf<FileNotFoundException>());
                That(() => { new StateInfo("Resources\\GameFile\\errorFile.txt"); }, Throws.TypeOf<ParseException>());
            });
        }
    }
}
