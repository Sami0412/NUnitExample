using Loans.Domain.Applications;
using NUnit.Framework;

namespace Loans.Tests;
[Category("Product Comparison")]
public class ProductComparerShould
{
    private List<LoanProduct> products;
    private ProductComparer sut;

    [OneTimeSetUp] // Runs ONCE before the first test in the class
    //Can speed tests up
    //Simulate long setup init time for list of products
    //Assume this list will not be modified by any tests as this would potentially break other tests (i.e.breaks test isolation)
    public void OneTimeSetUp()
    {
        products = new List<LoanProduct>
        {
            new LoanProduct(1, "a", 1),
            new LoanProduct(2, "b", 2),
            new LoanProduct(3, "c", 3)
        };
    }

    [OneTimeTearDown] //Runs ONCE after the last test executes
    public void OneTimeTearDown()
    {
        //e.g. disposing of shared expensive setup performed in one time setup
        //products.Dispose(); - (only works if products implements IDisposable)
    }
    
    [SetUp] //Runs before EACH test
    public void Setup()
    {
        // products = new List<LoanProduct>
        // {
        //     new LoanProduct(1, "a", 1),
        //     new LoanProduct(2, "b", 2),
        //     new LoanProduct(3, "c", 3)
        // };

        sut = new ProductComparer(new LoanAmount("USD", 200_000m), products);
    }

    [TearDown] 
    public void TearDown()
    {
        //Run after EACH test executes
        //sut.Dispose(); (only works if products implements IDisposable)
    }
    //Asserting against collections:
    //Check they contain the required number of items - ReturnCorrectNumberOfComparisons()
    //Assert that all items in collection are unique - NotReturnDuplicateComparisons()
    //Assert that a collection contains a specific item - ReturnComparisonForFirstProduct()
    //Assert against expected contents even if we don't know all the expected properties of the collection item - ReturnComparisonForFirstProductWithPartialKnownExpectedValues()
    [Test]
    //[Category("Product Comparison")]
    //[Category("Xyz")]
    //To run tests in a certain category- click Group By (Rider) & Category, Right click category & run tests
    //Tests can belong to more than one category - List categories om top of each other
    //Apply category attribute at test class level (above public class ...{}) to apply it to ALL tests in the class
    public void ReturnCorrectNumberOfComparisons()
    {
        List<MonthlyRepaymentComparison> comparisons = sut.CompareMonthlyRepayments(new LoanTerm(30));

        Assert.That(comparisons, Has.Exactly(3).Items);
    }
    
    [Test]
    public void NotReturnDuplicateComparisons()
    {
        List<MonthlyRepaymentComparison> comparisons = sut.CompareMonthlyRepayments(new LoanTerm(30));
        //Check that all items are unique - e.g. if we add another new LoanProduct(3, "c", 3) to products the test will fail
        Assert.That(comparisons, Is.Unique);
    }
    
    [Test]
    public void ReturnComparisonForFirstProduct()
    {
        List<MonthlyRepaymentComparison> comparisons = sut.CompareMonthlyRepayments(new LoanTerm(30));
        
        //Need to also know the expected monthly repayment to create an expected product
        var expectedProduct = new MonthlyRepaymentComparison("a", 1, 643.28m);
        
        Assert.That(comparisons, Does.Contain(expectedProduct));
    }
    
    [Test]
    public void ReturnComparisonForFirstProductWithPartialKnownExpectedValues()
    {
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