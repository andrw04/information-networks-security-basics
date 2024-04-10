using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace ObfuscateApp
{
    public class VariableRenamer : CSharpSyntaxRewriter
    {
        private Dictionary<string, string> _variableMap = new Dictionary<string, string>();
        private Random _random = new Random();

        public override SyntaxNode? VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            foreach (var v in node.Declaration.Variables)
            {
                string oldName = v.Identifier.ValueText;
                string newName = StringDictionary.GetRandom();
                _variableMap[oldName] = newName;
            }

            return base.VisitLocalDeclarationStatement(node);
        }

        public override SyntaxToken VisitToken(SyntaxToken token)
        {
            if (token.IsKind(SyntaxKind.IdentifierToken) && _variableMap.ContainsKey(token.ValueText))
                return SyntaxFactory.Identifier(token.LeadingTrivia, _variableMap[token.ValueText], token.TrailingTrivia);

            return base.VisitToken(token);
        }
    }
}
