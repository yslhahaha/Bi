//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class TB_SYS_DIR
    {
        public string DIR_ID { get; set; }
        public string PARENT_ID { get; set; }
        public byte ENABLED { get; set; }
        public string DIR_VIEW { get; set; }
        public string DIR_NAME { get; set; }
        public string DIR_URL { get; set; }
        public byte DIR_TYPE { get; set; }
        public string MEMO { get; set; }
        public Nullable<short> SORT_NO { get; set; }
        public byte DELETED { get; set; }
        public Nullable<decimal> IS_GROUP { get; set; }
        public Nullable<decimal> D_LEVEL { get; set; }
    }
}
