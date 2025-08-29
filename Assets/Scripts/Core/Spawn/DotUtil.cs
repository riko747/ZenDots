using Entities.Dot;

namespace Core.Spawn
{
    internal static class DotUtil
    {
        public static void Activate(Dot d)
        {
            if (d.IsPending) d.SetPendingState(false);
            d.SetActivatedState(true);
            d.SetLast(false);
            d.gameObject.SetActive(true);
        }
    }
}