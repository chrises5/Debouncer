using System;

namespace Debouncer
{
    public interface IInputSource<T> where T : Input
    {
        event EventHandler<T> InputRegisterd;
        event EventHandler<Exception> ExceptionRaised;
    }
}