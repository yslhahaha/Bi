﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bi.Domain
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OraDb104 : DbContext
    {
        public OraDb104()
            : base("name=OraDb104")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<TB_ADMIN_USER> TB_ADMIN_USER { get; set; }
        public virtual DbSet<TB_SYS_LOG> TB_SYS_LOG { get; set; }
        public virtual DbSet<TB_SYS_ROLE> TB_SYS_ROLE { get; set; }
        public virtual DbSet<TB_USER_ROLE> TB_USER_ROLE { get; set; }
        public virtual DbSet<TB_ROLE_DIR> TB_ROLE_DIR { get; set; }
        public virtual DbSet<TB_SYS_DIR> TB_SYS_DIR { get; set; }
    }
}
