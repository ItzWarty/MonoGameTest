using Dargon.PortableObjects;

namespace Common {
   public class CommonPofContext : PofContext {
      private const int kBasePofId = 4000000;

      public CommonPofContext() {
         RegisterPortableObjectType(kBasePofId + 0, typeof(GamePadStateDto));
      }
   }
}