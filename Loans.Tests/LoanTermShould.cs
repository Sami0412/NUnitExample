using System.ComponentModel.DataAnnotations;
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
            //third param of That() takes custom error message string
            //Adding msgs to every test can be a lot of overhead - name of test should be enough to desrcibe the error
            Assert.That(numberOfMonths, Is.EqualTo(12), "Months should be 12 * number of years");
        }

        [Test]
        public void StoreYears()
        {
            var sut = new LoanTerm(1);
            //There are several overloads of That() method
            Assert.That(sut.Years, Is.EqualTo(1));
            //Is is an abstract class containing static methods & properties
        }

        [Test]
        public void RespectValueEquality()
        {
            //LoanTerm inherits from ValueObject base CLASS - classes are reference types so usually 2 instances
            //would not be equal. See in ValueObject that the Equals method has been overridden. If you comment this out 
            //(with GetHashCode method) the assert will fail as they are different references to the same object
            var a = new LoanTerm(1);
            var b = new LoanTerm(1);
            
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void RespectValueInequality()
        {
            var a = new LoanTerm(1);
            var b = new LoanTerm(2);
            
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void ReferenceEqualityExample()
        {
            var a = new LoanTerm(1);
            var b = a;
            var c = new LoanTerm(1);
            //SameAs() checks that both variables point to the same object in memory - only concerned with references and not values
            Assert.That(a, Is.SameAs(b));
            Assert.That(a, Is.Not.SameAs(c));
            
            //List<T> is a reference type
            var x = new List<string> { "a", "b" };
            var y = x;
            var z = new List<string> {"a", "b"};
            
            Assert.That(y, Is.SameAs(x));
            Assert.That(z, Is.Not.SameAs(x)); //SameAs() constraint is only concerned with comparing references, not values
        }

        [Test]
        public void Double()
        {
            double a = 1.0 / 3.0;
            
            //Assert.That(a, Is.EqualTo(0.33) =>  fails because exact answer is 0.33333333331d
            //Within() method allows us to specify a tolerance that NUnit will use to compare the values
            Assert.That(a, Is.EqualTo(0.33).Within(0.004));
            //Can also specify a percentage tolerance with the Percent Modifier
            Assert.That(a, Is.EqualTo(0.33).Within(10).Percent);
        }

        [Test]
        public void NotAllowZeroYears()
        {
            //Assert.That(() => new LoanTerm(0), Throws.TypeOf<ArgumentOutOfRangeException>());
            
            Assert.That(() => new LoanTerm(0), Throws.TypeOf<ArgumentOutOfRangeException>()
                .With
                .Property("Message")
                .EqualTo("Please specify a value greater than 0. (Parameter 'years')"));
            //Hard-typed property name
            
            //More type-safe to use built in NUnit Message modifier:
            Assert.That(() => new LoanTerm(0), Throws.TypeOf<ArgumentOutOfRangeException>()
                .With
                .Message
                .EqualTo("Please specify a value greater than 0. (Parameter 'years')"));
            
            //If we don't care about the message but want to check that an ArgumentOutOfRangeException is thrown:
            Assert.That(() => new LoanTerm(0), Throws.TypeOf<ArgumentOutOfRangeException>()
                .With
                .Property("ParamName")
                .EqualTo("years"));
            //Not type safe

            //Use Matches() method and specify a predicate as a lambda expression
            Assert.That(() => new LoanTerm(0), Throws.TypeOf<ArgumentOutOfRangeException>()
                .With
                .Matches<ArgumentOutOfRangeException>(
                    ex => ex.ParamName == "years"));
        }
    }
}
