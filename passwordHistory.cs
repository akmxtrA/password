//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace password
{
    using System;
    using System.Collections.Generic;
    
    public partial class passwordHistory
    {
        public int id { get; set; }
        public Nullable<int> id_user { get; set; }
        public string password { get; set; }
    
        public virtual users users { get; set; }
    }
}
