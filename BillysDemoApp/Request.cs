namespace BillysDemoApp;

public record Request(string Code, string MethodName, List<object> Arguments);
