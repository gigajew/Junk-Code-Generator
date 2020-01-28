using ScintillaNET;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JCG
{
    public partial class Form1 : Form
    {
        private Type[] types = new Type[] {
                typeof(int),
                typeof(string),
                typeof(long),
                typeof(ulong),
                typeof(uint),
                typeof(List<string>),
                typeof(bool)
            };

        private Random random = new Random(Guid.NewGuid().GetHashCode());
        private Assembly mscorlib;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mscorlib = Assembly.ReflectionOnlyLoadFrom(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "mscorlib.dll"));

            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            //scintilla1.StyleResetDefault();
            //scintilla1.Styles[Style.Default].Font = "Consolas";
            //scintilla1.Styles[Style.Default].Size = 10;
            scintilla1.Lexer = Lexer.Cpp;
            //scintilla1.StyleClearAll();

            //// Configure the CPP (C#) lexer styles
            scintilla1.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            scintilla1.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla1.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla1.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            scintilla1.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            scintilla1.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            scintilla1.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
            scintilla1.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla1.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla1.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla1.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
            scintilla1.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            scintilla1.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
        }

        private void junkClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string random_namespace = string.Format("{0}.{1}", GetRandomClassName(), GetRandomClassName());
            string random_class_name = GetRandomClassName();

            SyntheticClass my_generated_class = new SyntheticClass(random_namespace, random_class_name);

            for (int i = 0; i < random.Next(8, 16); i++)
            {
                string random_property_name = GetRandomPropertyName();
                Type random_type = GetRandomType();

                SyntheticProperty property = my_generated_class.AddProperty(random_type, random_property_name);
                int r = random.Next(0, 3);
                if (r == 1)
                {
                    property.Modifier = SyntheticModifier.Public | SyntheticModifier.Static  ;
                } else if (r == 2)
                {
                    property.Modifier = SyntheticModifier.Public;
                } else
                {
                    property.Modifier = SyntheticModifier.Protected;
                }
            }

            // generate code to string
            string generated_code = my_generated_class.ToString();

            // add code to text
            scintilla1.Text = "";
            scintilla1.AddText(generated_code);
        }

        private Type GetRandomType()
        {
            return types[random.Next(0, types.Length)];
        }

        private string GetRandomPropertyName()
        {
            Type[] types = mscorlib.GetTypes();
            Type[] typesWithProperties = types.Where(type => type.GetProperties().Length > 0).ToArray();
            Type selectedType = typesWithProperties[random.Next(0, typesWithProperties.Length)];
            PropertyInfo[] properties = selectedType.GetProperties();
            return properties[random.Next(0, properties.Length)].Name;

        }

        private string GetRandomClassName()
        {
            Type[] types = mscorlib.GetTypes();
            string typeName = GetTypeName(types[random.Next(0, types.Length)], true);
            return typeName;
        }

        private string GetTypeName(Type type, bool lastOnly)
        {
            string name = "";
            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");
            CodeTypeReferenceExpression typeReferenceExpression = new CodeTypeReferenceExpression(new CodeTypeReference(type));
            using (StringWriter writer = new StringWriter())
            {
                codeDomProvider.GenerateCodeFromExpression(typeReferenceExpression, writer, new CodeGeneratorOptions());
                name = writer.GetStringBuilder().ToString();
            }
            if (lastOnly)
            {
                int index = name.LastIndexOf('.');
                if (index == -1)
                    return name;
                index++;
                int length = name.Length - index;

                return name.Substring(index, length);
            }
            else
            {
                return name;
            }
        }
    }
}
