using Common;
using Dargon.Courier;
using Dargon.Ryu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGameTest {
   public static class SurfaceProgram {
      public static void Main(string[] args) {
         Console.Title = "Surface";

         var ryu = new RyuFactory().Create();
         ((RyuContainerImpl)ryu).Setup(true);

         const int kPort = 21337;
         var courierClientFactory = ryu.Get<CourierClientFactory>();
         var courierClient = courierClientFactory.CreateUdpCourierClient(kPort, new CourierClientConfiguration {
            Identifier = Guid.NewGuid()
         });
         while (true) {
            var state = GetGamePadState();

            Console.WriteLine("Sending state: " + state);
            courierClient.SendBroadcast(state);
         }
      }

      private static GamePadStateDto GetGamePadState() {
         var state = GamePad.GetState(PlayerIndex.One);
         var stateDto = new GamePadStateDto(
            state.ThumbSticks.Left.X,
            state.ThumbSticks.Left.Y,
            state.ThumbSticks.Right.X,
            state.ThumbSticks.Right.Y,
            new[] {
               state.IsButtonDown(Buttons.A),
               state.IsButtonDown(Buttons.B),
               state.IsButtonDown(Buttons.X),
               state.IsButtonDown(Buttons.Y)
            });
         return stateDto;
      }
   }
}
