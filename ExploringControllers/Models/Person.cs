using ExploringControllers;
using System.Collections.Generic;

namespace ExploringControllers
{
    public class Person : IHasSysUser
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string SysUser { get; set; }
    }
}
