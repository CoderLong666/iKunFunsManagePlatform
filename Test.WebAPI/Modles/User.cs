using System.ComponentModel.DataAnnotations;

namespace Test.WebAPI.Modles
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class DisplayAttribute : Attribute
    {
        public string Name { get; set; } = "";
    }

    /// <summary>
    /// 记录状态（0:启用, 1:禁用）
    /// </summary>
    [Display(Name = "记录状态")]
    public enum EntityStatus
    {
        [Display(Name = "启用")] Enable = 0,
        [Display(Name = "禁用")] Disable = 1,
    }

    public class User : Entity
    {
        /// <summary>
        /// 用户名（唯一，登录用）
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// 姓名
        /// </summary>
        public string LegalName { get; set; } = "";

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; } = "";

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// 身份角色
        /// </summary>
        public string Role { get; set; } = "";

        /// <summary>
        /// 用户状态
        /// </summary>
        public EntityStatus Status { get; set; }
    }
}
