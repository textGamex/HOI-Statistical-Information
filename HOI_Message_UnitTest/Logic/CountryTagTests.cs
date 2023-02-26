using HOI_Message.Logic;
using Throws = NUnit.Framework.Throws;

namespace HOI_Message_UnitTest.Logic
{
    [TestFixture]
    public class CountryTagTests
    {
        [Test]
        public void TestNewCountryTag()
        {
            var emptyCountryTag = new CountryTag();
            var countryTag = new CountryTag("TAG");

            Multiple(() =>
            {
                That(countryTag.Tag, Is.EqualTo("TAG"));
                That(emptyCountryTag.Tag, Is.EqualTo(string.Empty));
            });
        }

        [Test]
        public void TestEquals()
        {
            var tag = new CountryTag("123");
            var IEqual = (IEquatable<CountryTag>)tag;
            var equalTag = new CountryTag("123");
            var equalNotTag = new CountryTag("000");
            object? obj = null;

            Multiple(() =>
            {
                That(tag.Equals(equalTag), Is.True);
                That(tag.Equals(obj), Is.False);
                That(IEqual.Equals(equalTag), Is.True);
                That(tag.Equals(equalNotTag), Is.False);
            });
        }

        [Test]
        public void TestThrowException()
        {
            Multiple(() =>
            {
                That(() => { new CountryTag("1234"); }, Throws.TypeOf<ArgumentException>());
                That(() => { new CountryTag("12"); }, Throws.TypeOf<ArgumentException>());
                That(() => { new CountryTag("123"); }, Throws.Nothing);
                That(() => { new CountryTag("12"); }, Throws.TypeOf<ArgumentException>());
                That(() => { new CountryTag("1234"); }, Throws.TypeOf<ArgumentException>());
                That(() => { new CountryTag("123"); }, Throws.Nothing);
            });
        }

        [Test]
        public void TestHashCode()
        {
            var tag = new CountryTag("tag");
            var equalTag = new CountryTag("tag");
            var equalNotTag = new CountryTag("000");

            Multiple(() =>
            {
                That(tag.GetHashCode(), Is.EqualTo(equalTag.GetHashCode()));
                That(tag.GetHashCode(), Is.Not.EqualTo(equalNotTag.GetHashCode()));
            });
        }

        [Test]
        public void TestOperator()
        {
            var tag = new CountryTag("tag");
            var equalTag = new CountryTag("tag");
            var equalNotTag = new CountryTag("000");

            Multiple(() =>
            {
                That(tag == equalTag, Is.True);
                That(tag == equalNotTag, Is.False);
                That(tag != equalTag, Is.False);
                That(tag != equalNotTag, Is.True);
            });
        }
    }
}
