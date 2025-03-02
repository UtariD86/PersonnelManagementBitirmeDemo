﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zurafworks.Shared.Entities.Abstract
{
    public abstract class EntityBase
    {
        public virtual int Id { get; set; }
        public virtual string CreatedByName { get; set; } = "Null";
        public virtual DateTime CreatedDate { get; set; } = DateTime.Now;
        public virtual DateTime ModifiedDate { get; set; } = DateTime.Now;
        public virtual bool IsDeleted { get; set; } = false;
        public virtual string ModifiedByName { get; set; } = "Admin";
        public virtual string? Note { get; set; } = "Null";
        public virtual bool IsActive { get; set; } = true;
    }
}
