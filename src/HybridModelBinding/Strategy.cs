using System.Linq;

namespace HybridModelBinding
{
    public static class Strategy
    {
        public static bool FirstInWins(
            string[] previouslyBoundValueProviderIds,
            string[] allValueProviderIds)
        {
            return !previouslyBoundValueProviderIds.Any();
        }

        public static bool Passthrough(
            string[] previouslyBoundValueProviderIds,
            string[] allValueProviderIds)
        {
            return true;
        }
    }
}
