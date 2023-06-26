namespace QueryPack.ModelManager
{
    using System.Text;
    using Humanizer;

    public class CSharpContextGenerator
    {
        private static List<string> _includes = new List<string>
        {
            "using System;",
            "using System.ComponentModel.DataAnnotations;",
            "using System.ComponentModel.DataAnnotations.Schema;",
            "using System.Collections.Generic;",
            "using Microsoft.EntityFrameworkCore;",
        };

        public string Generate(string @namespace, string contextClsName, string contextBaseClsName, IEnumerable<string> clsNames)
        {
            var builder = new StringBuilder();
            foreach (var include in _includes)
            {
                builder.AppendLine(include);
            }
            builder.AppendLine();
            builder.AppendLine($"namespace {@namespace}");
            builder.AppendLine("{");
            builder.AppendLine($"public class {contextClsName} : {contextBaseClsName}");
            builder.AppendLine("{");


            foreach (var clsName in clsNames)
            {
                builder.AppendLine($"public DbSet<{clsName}> {clsName.Pluralize()}" + " { get; set; }");
            }

            builder.AppendLine("}");
            builder.AppendLine("}");

            return builder.ToString();
        }
    }
}