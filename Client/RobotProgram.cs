using Common;
using Dargon.Courier;
using Dargon.Courier.Messaging;
using Dargon.Ryu;
using System;
using System.Threading;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace Client {
   public static class RobotProgram {
      public static void Main(string[] args) {
         InitializeLogging();

         var ryu = new RyuFactory().Create();
         ((RyuContainerImpl)ryu).Setup(true);

         const int kPort = 21337;
         var courierClientFactory = ryu.Get<CourierClientFactory>();
         var courierClient = courierClientFactory.CreateUdpCourierClient(kPort,
            new CourierClientConfiguration {
               Identifier = Guid.NewGuid()
            });
         courierClient.RegisterPayloadHandler<GamePadStateDto>(HandleGamePadState);

         new ManualResetEvent(false).WaitOne();
      }

      private static void HandleGamePadState(IReceivedMessage<GamePadStateDto> x) {
         var state = x.Payload;
         Console.WriteLine($"Got gamepad state: {state}");
      }

      private static void InitializeLogging() {
         var config = new LoggingConfiguration();
         Target debuggerTarget = new DebuggerTarget() {
            Layout = "${longdate}|${level}|${logger}|${message} ${exception:format=tostring}"
         };
         Target consoleTarget = new ColoredConsoleTarget() {
            Layout = "${longdate}|${level}|${logger}|${message} ${exception:format=tostring}"
         };

#if !DEBUG
         debuggerTarget = new AsyncTargetWrapper(debuggerTarget);
         consoleTarget = new AsyncTargetWrapper(consoleTarget);
#else
         AsyncTargetWrapper a; // Placeholder for optimizing imports
#endif

         config.AddTarget("debugger", debuggerTarget);
         config.AddTarget("console", consoleTarget);

         var debuggerRule = new LoggingRule("*", LogLevel.Trace, debuggerTarget);
         config.LoggingRules.Add(debuggerRule);

         var consoleRule = new LoggingRule("*", LogLevel.Trace, consoleTarget);
         config.LoggingRules.Add(consoleRule);

         LogManager.Configuration = config;
      }
   }
}
