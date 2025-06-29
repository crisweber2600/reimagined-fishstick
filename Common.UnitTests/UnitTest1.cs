namespace Common.UnitTests;

public class MathProcessorTests
{
    [Fact]
    public void Sum_UsesCalculator()
    {
        var calcMock = new Moq.Mock<ICalculator>();
        calcMock.Setup(m => m.Add(2, 3)).Returns(5);

        var processor = new MathProcessor(calcMock.Object);

        var result = processor.Sum(2, 3);

        Xunit.Assert.Equal(5, result);
        calcMock.Verify(m => m.Add(2, 3), Moq.Times.Once);
    }
}
