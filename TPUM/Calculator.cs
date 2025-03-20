namespace TPUM;

public class Calculator
{
    public int Add(int x, int y)
    {
        return x + y;
    }

    public int Subtract(int x, int y)
    {
        return x - y;
    }

    public int Multiply(int x, int y)
    {
        return x * y;
    }

    public int Divide(int x, int y)
    {
        if (y != 0)
            return x / y;

        throw new ArgumentException("You cannot divide by zero!");
    }
}