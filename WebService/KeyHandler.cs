using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebService
{
    internal class KeyHandler
    {
        private static List<string> ValidKeys = new(){ "7289B39D-8BF4-43E8-982A-CFFA518E04BD" };
        internal bool isKeyValid(string key)
        {
            if (ValidKeys.Contains(key)) return true;
            return false;
        }
    }
}
