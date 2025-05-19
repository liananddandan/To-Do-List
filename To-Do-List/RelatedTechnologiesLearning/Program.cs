// See https://aka.ms/new-console-template for more information

// proxy dynamic

using Castle.DynamicProxy;
using RelatedTechnologiesLearning.DynamicProxy;

var proxyGenerator = new ProxyGenerator();
var calculator = new Calculator();
var interceptor = new LoggingInterceptor();

ICalculator proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ICalculator>(calculator, interceptor);

int sum = proxy.Add(3, 4);
int product = proxy.Multiply(3, 4);