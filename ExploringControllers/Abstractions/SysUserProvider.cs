using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExploringControllers
{
    public class SysUserProvider : ISysUserProvider
    {
        /// <summary>
        /// hard code for demo purposes
        /// </summary>
        public string SysUser { get; set; } = "maria@a.test";
    }
}
