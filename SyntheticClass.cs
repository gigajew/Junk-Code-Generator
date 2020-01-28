using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCG
{
    public class SyntheticClass
    {
        public string Name
        {
            get { return _name; }
            set { if (IsValidName(value))
                {
                    _name = value;
                } else
                {
                    throw new FormatException("illegal class name");
                }
            }
        }

        public string Namespace
        {
            get { return _namespace; }
            set
            {
                if (IsValidNamespace(value))
                {
                    _namespace = value;
                }
                else
                {
                    throw new FormatException("illegal namespace name");
                }
            }
        }

        public List<SyntheticProperty > Properties
        {
get { return _properties; }
            set { _properties = value; }
        }

        public SyntheticClass(string @namespace , string name )
        {
            Namespace = @namespace;
            Name = name;
            Properties = new List<SyntheticProperty>();
        }

        public SyntheticProperty AddProperty(Type type, string name)
        {
            SyntheticProperty property = new SyntheticProperty(this, name, type);
            Properties.Add(property);
            return property;
        }

        public override string ToString()
        {
            int ident = 0;

            StringBuilder s = new StringBuilder();
            s.AppendLine("using System;");
            s.AppendLine("using System.IO;");
            s.AppendLine("using System.Collections.Generic;");

            AppendFormatLine(s, ident, "namespace {0}", Namespace);
            AppendBracket(s, ident++, false);

            AppendFormatLine(s, ident, "public class {0}", Name);
            AppendBracket(s, ident++, false);

            foreach(SyntheticProperty property in Properties)
            {
                if(property.Modifier.HasFlag(SyntheticModifier.Static))
                {
                    AppendFormatLine(s, ident, "public static {0} {1}", GetTypeName(property.Type, true), property.Name);

                } else
                {
                    AppendFormatLine(s, ident, "public {0} {1}", GetTypeName(property.Type, true), property.Name);

                }
                AppendBracket(s, ident++, false);
                if(property.Modifier.HasFlag(SyntheticModifier.Protected ))
                {
                    AppendFormatLine(s, ident, "get; protected set;");
                } else if (property.Modifier.HasFlag(SyntheticModifier.Private ))
                {
                    AppendFormatLine(s, ident, "get; private set;");
                } else
                {
                    AppendFormatLine(s, ident, "get; set;");
                }
                
                AppendBracket(s, --ident, true);
            }

            AppendBracket(s, --ident, true);
            AppendBracket(s, --ident, true);


            return s.ToString();
        }

        private static void AppendFormatLine(StringBuilder builder, int ident, string line, params string [] parameters )
        {
            for (int i = 0; i < ident; i++)
                builder.Append("\t");
            builder.AppendFormat(line, parameters);
            builder.AppendLine();

        }


        private static void AppendLine(StringBuilder builder, int ident, string line )
        {
            for (int i = 0; i < ident; i++)
                builder.Append("\t");
            builder.AppendLine(line);
        }

        private static void AppendBracket(StringBuilder builder, int ident, bool left )
        {
            AppendLine(builder, ident, left ? "}" : "{");
        }

        public bool IsValidName(string name)
        {
            return char.IsLetter(name[0]) || name[0] == '_';
        }

        public bool IsValidNamespace(string name)
        {
            return char.IsLetter(name[0]);
        }

        private string GetTypeName(Type type, bool lastOnly)
        {
            string name = "";
            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");
            CodeTypeReferenceExpression typeReferenceExpression = new CodeTypeReferenceExpression(new CodeTypeReference(type));
            using (StringWriter writer = new StringWriter())
            {
                codeDomProvider.GenerateCodeFromExpression(typeReferenceExpression, writer, new CodeGeneratorOptions());
                name =  writer.GetStringBuilder().ToString();
            }
            if(lastOnly )
            {
                int index = name.LastIndexOf('.');
                if (index == -1)
                    return name;
                index++;
                int length = name.Length - index;

                return name.Substring(index, length);
            }else
            {
                return name;
            }
        }

        private string _name;
        private string _namespace;
        private List<SyntheticProperty> _properties;
    }
}
