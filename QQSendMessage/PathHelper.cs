using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQSendMessage
{
    public class PathHelper
    {

        public static string GetDir() 
        {
            var paths = AppContext.BaseDirectory.Split("\\", StringSplitOptions.RemoveEmptyEntries).ToList();
            paths.RemoveRange(paths.Count - 3, 3);
            return string.Join("\\", paths);
        }
    }
}
