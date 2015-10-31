using Common;
using Dargon.Courier;
using Dargon.Courier.Messaging;
using Dargon.Ryu;
using System;
using System.Threading;

namespace Client {
   public static class RobotProgram {
      public static void Main(string[] args) {
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
   }
}
