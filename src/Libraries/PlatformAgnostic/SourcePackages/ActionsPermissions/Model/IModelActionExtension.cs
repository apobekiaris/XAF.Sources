using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAF.ActionsPermissions.Model
{
    public interface IModelActionExtension
    {
        [DefaultValue(false)]
        [Category("Permissions")]
        bool EnablePermissions { get; set; }
    }
}
