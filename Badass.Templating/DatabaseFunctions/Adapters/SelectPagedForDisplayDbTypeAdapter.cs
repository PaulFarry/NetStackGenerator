﻿using System.Collections.Generic;
using Badass.Model;
using Badass.Templating.DatabaseFunctions.Adapters.Fields;

namespace Badass.Templating.DatabaseFunctions.Adapters
{
    public class SelectPagedForDisplayDbTypeAdapter : SelectForDisplayDbTypeAdapter
    {
        public SelectPagedForDisplayDbTypeAdapter(ApplicationType applicationType, Domain domain) : base(applicationType, "select_paged_for_display", domain)
        {
        }
        
        public override List<IPseudoField> SelectInputFields
        {
            get
            {
                var fields = base.SelectInputFields;
                fields.Add(PageSizeField);
                fields.Add(PageNumberField);
                fields.Add(SortField);
                fields.Add(SortDescendingField);
                return fields;
            }
        }

        public IPseudoField PageSizeField => new PageSizeField();

        public IPseudoField PageNumberField => new PageNumberField();

        public IPseudoField SortField => new SortField();

        public IPseudoField SortDescendingField => new SortDescendingField();
    }
}