namespace ExploringControllers
{
    public class Activity : IHasSysUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SysUser { get; set; }
    }
}
