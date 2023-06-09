﻿using System.Collections.Generic;
using Smartstore.Core.DataExchange;
using Smartstore.Web.Modelling;

namespace Smartstore.Admin.Models.Import
{
    public partial class ImportProfileListModel : EntityModelBase
    {
        [LocalizedDisplay("Admin.Common.Entity")]
        public ImportEntityType EntityType { get; set; }

        public List<ImportProfileModel> Profiles { get; set; } = new();
    }
}
