using Castle.DynamicProxy;

namespace RelatedTechnologiesLearning.DynamicProxy;

public class LoggingInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine($"[Before]Intercepted in {invocation.Method.Name}");
        invocation.Proceed();
        Console.WriteLine($"[After]Intercepted in {invocation.Method.Name}, return {invocation.ReturnValue}");
    }
}