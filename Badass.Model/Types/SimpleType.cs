﻿using System.Collections.Generic;
using System.Linq;

namespace Badass.Model
{
    public class SimpleType
    {
        public const string DefaultNamespace = "public";
        private string _ns;
        
        public SimpleType(string name, string ns, Domain domain)
        {
            Name = name;
            _ns = ns;
            Fields = new List<Field>();
            Domain = domain;
        }

        public string Name { get; }

        public string Namespace {             
            get
            {
                if (!string.IsNullOrEmpty(Domain.DefaultNamespace) && (string.IsNullOrEmpty(_ns) || _ns == DefaultNamespace))
                {
                    return Domain.DefaultNamespace;
                }

                return _ns;
            } 
        }
        
        public Domain Domain { get; }

        public List<Field> Fields { get; }

        // TODO - this assumes the first text field is the best "summary" of a thing
        // which may not be the case. We could use some attributes to control which field(s) make up the 
        // "summary" for a related type
        public Field DisplayField => Fields.OrderBy(f => f.Rank).FirstOrDefault(f => f.ClrType == typeof(string));

        public dynamic Attributes { get; set; }
    }
}