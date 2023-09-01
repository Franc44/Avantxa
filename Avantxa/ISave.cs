using System;
using System.IO;
namespace Avantxa
{
    public interface ISave
    {
        string Save(MemoryStream fileStream);
    }
}
