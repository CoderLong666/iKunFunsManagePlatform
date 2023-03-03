namespace Test.WebAPI.Modles
{
    public class People : Entity
    {
        /// <summary>
        /// 人员姓名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; } = string.Empty;

        /// <summary>
        /// 出生时间
        /// </summary>
        public DateTime Birth { get; set; }

    }
}
