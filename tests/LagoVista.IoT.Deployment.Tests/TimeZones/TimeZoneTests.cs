
using LagoVista.IoT.Deployment.Admin.Managers;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.TimeZones
{
    public class TimeZoneTests
    {
        [Test]
        public void GetTimeZones_When_Called_Should_Return_TimeZones()
        {
            var sut = new TimeZoneService();

            var result = sut.GetTimeZones();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void GetTimeZones_When_Loaded_Should_Have_Unique_IntIds()
        {
            var sut = new TimeZoneService();

            var references = sut.GetTimeZoneReferences();

            var duplicateIds = references
                .GroupBy(tz => tz.IntId)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            Assert.That(duplicateIds, Is.Empty, $"Duplicate IntIds found: {String.Join(',', duplicateIds)}");
        }


        [Test]
        public void GetTimeZones_When_Loaded_Should_Have_Unique_Ids()
        {
            var sut = new TimeZoneService();

            var references = sut.GetTimeZoneReferences();

            var duplicateIds = references
                .GroupBy(tz => tz.Id)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            Assert.That(duplicateIds, Is.Empty, $"Duplicate Ids found: {String.Join(',', duplicateIds)}");
        }

        [Test]
        public void GetTimeZoneReferenceByIntId_When_ValidId_Should_Return_TimeZone()
        {
            var sut = new TimeZoneService();

            var first = sut.GetTimeZoneReferences().First();

            var result = sut.GetTimeZoneReferenceByIntId(first.IntId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IntId, Is.EqualTo(first.IntId));
            Assert.That(result.Id, Is.EqualTo(first.Id));
        }
        [Test]
        public void GetTimeZoneByIntId_When_ValidId_Should_Return_TimeZoneInfo()
        {
            var sut = new TimeZoneService();

            var reference = sut.GetTimeZoneReferences().First();

            var result = sut.GetTimeZoneByIntId(reference.IntId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(reference.Id));
        }


        [Test]
        public void GetTimeZoneEnumOptions_When_Called_Should_Use_IntId_As_Id_And_Key()
        {
            var sut = new TimeZoneService();

            var references = sut.GetTimeZoneReferences();
            var firstReference = references.First();

            var options = sut.GetTimeZoneEnumOptions();
            var option = options.First(opt => opt.Id == firstReference.IntId.ToString());

            Assert.That(option.Key, Is.EqualTo(firstReference.IntId.ToString()));
            Assert.That(option.Label, Is.EqualTo(firstReference.DisplayName));
            Assert.That(option.Name, Is.EqualTo(firstReference.DisplayName));
        }

        [Test]
        public void GetTimeZoneReferenceByIntId_When_InvalidId_Should_Return_Null()
        {
            var sut = new TimeZoneService();

            Assert.That(() => sut.GetTimeZoneReferenceByIntId(Int32.MinValue), Throws.InvalidOperationException);
        }

        [Test]
        public void GetTimeZoneReferenceByIntId_When_InvalidId_Should_Throw()
        {
            var sut = new TimeZoneService();

            var ex = Assert.Throws<InvalidOperationException>(() => sut.GetTimeZoneReferenceByIntId(Int32.MinValue));

            Assert.That(ex.Message, Does.Contain("Unknown timezone IntId"));
        }
    }
}
