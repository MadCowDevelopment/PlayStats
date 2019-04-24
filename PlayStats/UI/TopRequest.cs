using DynamicData;

namespace PlayStats.UI
{
    public class TopRequest : IVirtualRequest
    {
        public TopRequest(int size)
        {
            Size = size;
        }

        public int Size { get; }
        public int StartIndex { get; } = 0;
    }
}