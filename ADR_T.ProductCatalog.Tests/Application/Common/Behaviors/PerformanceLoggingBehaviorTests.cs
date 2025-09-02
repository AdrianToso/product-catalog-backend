using MediatR;
using Microsoft.Extensions.Logging;
using ADR_T.ProductCatalog.Application.Common.Behaviors;

namespace ADR_T.ProductCatalog.Tests.Application.Common.Behaviors
{
    public class PerformanceLoggingBehaviorTests
    {
        private readonly TestLogger _testLogger;
        private readonly PerformanceLoggingBehavior<TestRequest, TestResponse> _behavior;

        public PerformanceLoggingBehaviorTests()
        {
            _testLogger = new TestLogger();
            _behavior = new PerformanceLoggingBehavior<TestRequest, TestResponse>(_testLogger);
        }

        [Fact]
        public async Task Handle_ShouldLogStartAndEnd_WhenRequestSucceeds()
        {
            // Arrange
            var request = new TestRequest();
            var response = new TestResponse();
            MediatR.RequestHandlerDelegate<TestResponse> next = (CancellationToken ct) => Task.FromResult(response);

            // Act
            var result = await _behavior.Handle(request, next, CancellationToken.None);

            // Assert
            Assert.Equal(response, result);
            Assert.Contains(_testLogger.LogEntries, e => e.Message.Contains("Inicio de"));
            Assert.Contains(_testLogger.LogEntries, e => e.Message.Contains("Fin de"));
        }

        [Fact]
        public async Task Handle_ShouldLogStartAndError_WhenRequestThrowsException()
        {
            // Arrange
            var request = new TestRequest();
            var exception = new Exception("Test exception");
            MediatR.RequestHandlerDelegate<TestResponse> next = (CancellationToken ct) => Task.FromException<TestResponse>(exception);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _behavior.Handle(request, next, CancellationToken.None));

            Assert.Contains(_testLogger.LogEntries, e => e.Message.Contains("Inicio de"));
            Assert.Contains(_testLogger.LogEntries, e => e.Message.Contains("Error en"));
        }

        // Implementación de TestLogger
        public class TestLogger : ILogger<PerformanceLoggingBehavior<TestRequest, TestResponse>>
        {
            public List<LogEntry> LogEntries { get; } = new List<LogEntry>();

            public IDisposable BeginScope<TState>(TState state) => new NullScope();

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var message = formatter(state, exception);
                LogEntries.Add(new LogEntry
                {
                    LogLevel = logLevel,
                    Message = message,
                    Exception = exception
                });
            }

            private class NullScope : IDisposable
            {
                public void Dispose() { }
            }
        }

        public class LogEntry
        {
            public LogLevel LogLevel { get; set; }
            public string Message { get; set; }
            public Exception Exception { get; set; }
        }

        // Clases de prueba internas
        public class TestRequest : IRequest<TestResponse> { }
        public class TestResponse { }
    }
}
