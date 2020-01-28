using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCG
{
    public class SyntheticProperty
    {
        public string Name
        {
            get { return _name; }
            set
            {
                if (IsValidName(value))
                {
                    _name = value;
                }
                else
                {
                    throw new FormatException("illegal property name");
                }
            }
        }

        public SyntheticClass ParentClass
        {
            get { return _parentClass; }
            set { _parentClass = value; }
        }

        public SyntheticModifier Modifier
        {
            get; set;
        }

        public Type Type { get; set; }

        public SyntheticProperty(SyntheticClass parentClass, string name, Type type)
        {
            _parentClass = parentClass;
            Name = name;
            Type = type;
        }

        public bool IsValidName(string name)
        {
            return char.IsLetter(name[0]) || name[0] == '_' &&
                   !string.Equals(name, _parentClass.Name , StringComparison.CurrentCultureIgnoreCase);
        }

        private string _name;
        private SyntheticClass _parentClass;
    }
}
