namespace TPUM.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void AddMethodTest()
    {
        int x = 12;
        int y = 13;

        Calculator c = new Calculator();

        Assert.AreEqual(25, c.Add(x, y));
    }

    [TestMethod]
    public void SubtractMethodTest()
    {
        int x = 12;
        int y = 11;

        Calculator c = new Calculator();

        Assert.AreEqual(1, c.Subtract(x, y));
    }

    [TestMethod]
    public void MultiplyMethodTest()
    {
        int x = 5;
        int y = 6;

        Calculator c = new Calculator();

        Assert.AreEqual(30, c.Multiply(x, y));
    }

    [TestMethod]
    public void DivideMethodTest()
    {
        int x = 10;
        int y = 5;

        Calculator c = new Calculator();

        Assert.AreEqual(2, c.Divide(x, y));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DivideByZeroMethodTest()
    {
        int x = 10;
        int y = 0;

        Calculator c = new Calculator();

        var result = c.Divide(x, y);
    }
}
