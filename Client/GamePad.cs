using Common;

namespace Client {
   public interface GamePad {
      float LeftX { get; }
   }

   public class RemoteGamePadImpl : GamePad {
      private GamePadStateDto lastState;

      public float LeftX => lastState.LeftX;
   }
}