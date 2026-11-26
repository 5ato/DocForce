namespace DocForce.Application.Common.Dispatcher.Interfaces;

public interface IRequest<out TRespone>;

public interface IRequest : IRequest<Unit>;
