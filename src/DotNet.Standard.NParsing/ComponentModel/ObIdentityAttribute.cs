using System;

namespace DotNet.Standard.NParsing.ComponentModel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ObIdentityAttribute : Attribute
    {
        /// <summary>
        /// 标识列名称
        /// </summary>
        public string IdentityName { get; }

        /// <summary>
        /// 标识列类型
        /// </summary>
        public ObIdentity ObIdentity { get; }

        /// <summary>
        /// 起始值
        /// </summary>
        public long Seed { get; }

        /// <summary>
        /// 增长量
        /// </summary>
        public int Increment { get; }

        public ObIdentityAttribute()
        {
            IdentityName = "";
            ObIdentity = ObIdentity.Program;
            Seed = 1;
            Increment = 1;
        }

        public ObIdentityAttribute(long seed, int increment)
        {
            IdentityName = "";
            ObIdentity = ObIdentity.Program;
            Seed = seed;
            Increment = increment;
        }

        public ObIdentityAttribute(string identityName)
        {
            IdentityName = identityName;
            ObIdentity = identityName.Equals("") ? ObIdentity.Program : ObIdentity.Database;
            Seed = 1;
            Increment = 1;
        }

        public ObIdentityAttribute(string identityName, long seed, int increment)
        {
            IdentityName = identityName;
            ObIdentity = identityName.Equals("") ? ObIdentity.Program : ObIdentity.Database;
            Seed = seed;
            Increment = increment;
        }

        public ObIdentityAttribute(ObIdentity obIdentity)
        {
            IdentityName = obIdentity.Equals(ObIdentity.Program) ? "" : "IdentityName";
            ObIdentity = obIdentity;
            Seed = 1;
            Increment = 1;
        }

        public ObIdentityAttribute(ObIdentity obIdentity, long seed, int increment)
        {
            IdentityName = obIdentity.Equals(ObIdentity.Program) ? "" : "IdentityName";
            ObIdentity = obIdentity;
            Seed = seed;
            Increment = increment;
        }

        public ObIdentityAttribute(ObIdentity obIdentity, string identityName)
        {
            IdentityName = identityName;
            ObIdentity = obIdentity;
            Seed = 1;
            Increment = 1;
        }

        public ObIdentityAttribute(ObIdentity obIdentity, string identityName, long seed, int increment)
        {
            IdentityName = identityName;
            ObIdentity = obIdentity;
            Seed = seed;
            Increment = increment;
        }
    }
}
