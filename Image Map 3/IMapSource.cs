using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMap
{
    public interface IMapSource
    {
        IReadOnlyDictionary<long, Map> GetMaps();
        IEnumerable<long> GetTakenIDs();
        void ChangeMapID(long from, long to);
        void AddMaps(IReadOnlyDictionary<long, Map> maps);
        void RemoveMaps(IEnumerable<long> ids);
    }
}
