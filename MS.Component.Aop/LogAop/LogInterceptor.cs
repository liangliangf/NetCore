

using Castle.DynamicProxy;

namespace MS.Component.Aop //Tip:命名空间
{
    public class LogInterceptor : IInterceptor
    {
        private readonly LogInterceptorAsync _logInterceptorAsync;

        public LogInterceptor(LogInterceptorAsync logInterceptorAsync)
        {
            _logInterceptorAsync = logInterceptorAsync;
        }

        public void Intercept(IInvocation invocation)
        {
            _logInterceptorAsync.ToInterceptor().Intercept(invocation);
        }
    }
}
