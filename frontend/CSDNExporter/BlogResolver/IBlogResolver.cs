using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDNExporter.BlogResolver
{
    public interface IBlogResolver
    {
        Task<string[]> Resolve(string[] content,object context);
    }
}
