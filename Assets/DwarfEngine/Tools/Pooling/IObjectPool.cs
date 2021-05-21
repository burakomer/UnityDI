using System.Collections;

namespace DwarfEngine.Tools.Pooling
{
    public interface IObjectPool : IEnumerable
    {
        bool Initialized { get; set; }
        void Initialize(object objectToPool, int amountToPool, bool expandInNeed, int expandAmount = 1);
        object GetPooledObject();
    }
}