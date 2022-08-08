using Loans.Domain.Applications;
using NUnit.Framework;

namespace Loans.Tests;

public class ProductComparerShould
{
    //Asserting against collections:
    //Check they contain the required number of items - ReturnCorrectNumberOfComparisons()
    //Assert that all items in collection are unique - NotReturnDuplicateComparisons()
    //Assert that a collection contains a specific item - ReturnComparisonForFirstProduct()
    //Assert against expected contents even if we don't know all the expected properties of the collection item - ReturnComparisonForFirstProductWithPartialKnownExpectedValues()
    
    [Test]
    public void ReturnCorrectNumberOfComparisons()
    {
        //Arrange
        var products = new List<LoanProduct>
        {
            new LoanProduct(1, "a", 1),
            new LoanProduct(2, "b", 2),
            new LoanProduct(3, "c", 3)
        };

        var sut = new ProductComparer(new LoanAmount("USD", 200_000m), products);
        //Act
        List<MonthlyRepaymentComparison> comparisons = sut.CompareMonthlyRepayments(new LoanTerm(30));
        //Assert
        
        Assert.That(comparisons, Has.Exactly(3).Items);
    }
    
    [Test]
    public void NotReturnDuplicateComparisons()
    {
        var products = new List<LoanProduct>
        {
            new LoanProduct(1, "a", 1),
            new LoanProduct(2, "b", 2),
            new LoanProduct(3, "c", 3),
        };

        var sut = new ProductComparer(new LoanAmount("USD", 200_000m), products);

        List<MonthlyRepaymentComparison> comparisons = sut.CompareMonthlyRepayments(new LoanTerm(30));
        //Check that all items are unique - e.g. if we add another new LoanProduct(3, "c", 3) to products the test will fail
        Assert.That(comparisons, Is.Unique);
    }
    
    [Test]
    public void ReturnComparisonForFirstProduct()
    {
        var products = new List<LoanProduct>
        {
            new LoanProduct(1, "a", 1),
            new LoanProduct(2, "b", 2),
            new LoanProduct(3, "c", 3),
        };

        var sut = new ProductComparer(new LoanAmount("USD", 200_000m), products);

        List<MonthlyRepaymentComparison> comparisons = sut.CompareMonthlyRepayments(new LoanTerm(30));
        
        //Need to also know the expected monthly repayment to create an expected product
        var expectedProduct = new MonthlyRepaymentComparison("a", 1, 643.28m);
        
        Assert.That(comparisons, Does.Contain(expectedProduct));
    }
    
    [Test]
    public void ReturnComparisonForFirstProductWithPartialKnownExpectedValues()
    {
        var products = new List<LoanProduct>
        {
            new LoanProduct(1, "a", 1),
            new LoanProduct(2, "b", 2),
            new LoanProduct(3, "c", 3),
        };

        var sut = new ProductComparer(new LoanAmount("USD", 200_000m), products);

        List<MonthlyRepaymentComparison> comparisons = sut.CompareMonthlyRepayments(new LoanTerm(30));
        
        //Don't care about the expected monthly repayment, only that the product is there
        //This checks that collection has exactly ONE item with the matching properties
        Assert.That(comparisons, Has.Exactly(1)
            .Property("ProductName").EqualTo("a")
            .And
            .Property("InterestRate").EqualTo(1)
            .And
            .Property("MonthlyRepayment").GreaterThan(0));
        //Hard typing property as a string is not ideal - if property name changes this test will break
        //More type-safe way of writing this test:
        Assert.That(comparisons, Has.Exactly(1)
            .Matches<MonthlyRepaymentComparison>(
                item => item.ProductName == "a" &&
                        item.InterestRate == 1 &&
                        item.MonthlyRepayment > 0));
        
    }
}