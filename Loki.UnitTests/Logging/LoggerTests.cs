using System;
using Loki.Interfaces.Logging;
using Loki.Server.Logging;
using Xunit;

namespace Loki.UnitTests.Logging
{
    public class LoggerTests
    {
        #region Fakes

        private class OnErrorException: Exception { }

        private enum CustomEnum
        {
            A,
            B
        }

        private struct CustomType { }

        #endregion

        [Fact]
        public void ErrorEventInvokesSubscribedFunction()
        {
            ILogger logger = new Logger();
            logger.OnError += (sender, args) => { Assert.Equal(typeof(OnErrorException), args.Exception.GetType()); };

            logger.Error(new OnErrorException());
        }

        [Fact]
        public void WarnEventInvokesSubscribedFunction()
        {
            ILogger logger = new Logger();
            logger.OnWarn += (sender, args) =>
            {
                Assert.Equal("WarnMessage", args.Message);
            };

            logger.Warn("WarnMessage");
        }

        [Fact]
        public void InfoEventInvokesSubscribedFunction()
        {
            ILogger logger = new Logger();
            logger.OnInfo += (sender, args) =>
            {
                Assert.Equal("InfoMessage", args.Message);
            };

            logger.Info("InfoMessage");
        }

        [Fact]
        public void DebugEventInvokesSubscribedFunction()
        {
            ILogger logger = new Logger();
            logger.OnDebug += (sender, args) =>
            {
                Assert.Equal(nameof(logger.OnDebug), args.Message);
            };

            logger.Debug(nameof(logger.OnDebug));
        }

        [Fact]
        public void CustomEventInvokesSubscribedFunction()
        {
            ILogger logger = new Logger();
            logger.OnCustom += (sender, args) =>
            {
                Assert.Equal("CUSTOM1", args.EventType);
                Assert.Equal("Message", args.Message);
            };

            logger.Custom("CUSTOM1", "Message");
        }

        [Fact]
        public void CustomEventInvokesSubscribedFunctionWithEnum()
        {
            ILogger logger = new Logger();
            logger.OnCustom += (sender, args) =>
            {
                Assert.Equal("B", args.EventType);
                Assert.Equal("Message", args.Message);
            };

            CustomEnum e = CustomEnum.B;
            logger.Custom(e, "Message");
        }

        [Fact]
        public void CustomEventInvokesSubscribedFunctionWithType()
        {
            ILogger logger = new Logger();
            logger.OnCustom += (sender, args) =>
            {
                Assert.Equal("CustomType", args.EventType);
                Assert.Equal("Message", args.Message);
            };
            
            logger.Custom(typeof(CustomType), "Message");
        }

        [Fact]
        public void LoggerRendersOutputWhenLogLevelIsCorrect()
        {
            int calls = 0;

            ILogger logger = new Logger();
            logger.OnInfo += (sender, args) =>
            {
                calls += 1;
            };
            logger.OnDebug+= (sender, args) =>
            {
                calls += 1;
            };

            logger.LogLevel = LogLevel.Info;
            logger.Info("Test");
            logger.Debug("Test");

            Assert.Equal(1, calls);
        }
    }
}
