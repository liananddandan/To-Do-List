namespace RelatedTechnologiesLearning.DynamicProxy;

public class Calculator: ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Multiply(int a, int b) => a * b;
}