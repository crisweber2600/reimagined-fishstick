namespace Common;

public interface ICalculator
{
    int Add(int a, int b);
}

public class Calculator : ICalculator
{
    public int Add(int a, int b) => a + b;
}

public class MathProcessor
{
    private readonly ICalculator _calculator;
    public MathProcessor(ICalculator calculator)
    {
        _calculator = calculator;
    }

    public int Sum(int a, int b)
    {
        return _calculator.Add(a, b);
    }
}
