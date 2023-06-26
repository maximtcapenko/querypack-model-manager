namespace QueryPack.ModelManager.Schema.Processing.Processors.Impl
{
    using Microsoft.CodeAnalysis;
    using Processing.Extensions;

    internal class TableNameSyntaxPredicateExpression
    {
        static string[] tableAttributes = { "Table", "TableAttribute" };
        private readonly string _tableName;

        public TableNameSyntaxPredicateExpression(string tableName)
        {
            _tableName = tableName;
        }

        public bool Apply(SyntaxTree syntax)
        {
            var classDeclaration = syntax.GetClassDeclaration();
            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                var tableAttribute = attributeList.Attributes.FirstOrDefault(e => tableAttributes.Contains(e.Name.ToString()));
                if (tableAttribute != null)
                {
                    var tableName = tableAttribute.ArgumentList.Arguments.FirstOrDefault();
                    return _tableName == tableName.GetText().ToString().Replace("\"", "");
                }
            }

            return false;
        }
    }
}
