using Loans.Domain.Applications;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Loans.Tests
{
    [TestFixture]
    public class LoanTermShould
    {
        [Test]
        public void ReturnTermInMonths()
        {
            //sut = system under test
            var sut = new LoanTerm(1);

            var numberOfMonths = sut.ToMonths();

            Assert.That(numberOfMonths, new EqualConstraint(12));
        }

        [Test]
        public void StoreYears()
        {
            var sut = new LoanTerm(1);
            
            Assert.That(sut.Years, Is.EqualTo(1));
        }
    }
}
