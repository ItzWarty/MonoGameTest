using Common;
using Dargon.Courier;
using Dargon.Ryu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace MonoGameTest {
   public static class SurfaceProgram {
      public static void Main(string[] args) {
         InitializeLogging();
         Console.Title = "Surface";

         var ryu = new RyuFactory().Create();
         ((RyuContainerImpl)ryu).Setup(true);

         const int kPort = 21337;
         var courierClientFactory = ryu.Get<CourierClientFactory>();
         var courierClient = courierClientFactory.CreateUdpCourierClient(kPort, new CourierClientConfiguration {
            Identifier = Guid.NewGuid()
         });

         int counter = 0;
         while (true) {
            var state = GetGamePadState(counter++);

            Console.WriteLine("Sending state: " + state);
            courierClient.SendBroadcast(state);
            Thread.Sleep(70);
         }
      }

      private static GamePadStateDto GetGamePadState(int packetId) {
         var state = GamePad.GetState(PlayerIndex.One);
         var stateDto = new GamePadStateDto(
            packetId,
            state.ThumbSticks.Left.X,
            state.ThumbSticks.Left.Y,
            state.ThumbSticks.Right.X,
            state.ThumbSticks.Right.Y,
            state.Triggers.Left,
            state.Triggers.Right,
            new[] {
               state.IsButtonDown(Buttons.A),
               state.IsButtonDown(Buttons.B),
               state.IsButtonDown(Buttons.X),
               state.IsButtonDown(Buttons.Y),
               state.IsButtonDown(Buttons.LeftShoulder),
               state.IsButtonDown(Buttons.RightShoulder),
               state.IsButtonDown(Buttons.Back),
               state.IsButtonDown(Buttons.Start),
               state.DPad.Left == ButtonState.Pressed,
               state.DPad.Right == ButtonState.Pressed,
               state.DPad.Up == ButtonState.Pressed,
               state.DPad.Down == ButtonState.Pressed
            });
         return stateDto;
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
