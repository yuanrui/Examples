using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace Study.CodeGenerator.Core
{
    public class CodeDomHelper
    {
        public CodeCompileUnit GetCodeCompileUnit(string nameSpace, string className, string comment = "")
        {
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace(nameSpace);
            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(className);
            if (!string.IsNullOrWhiteSpace(comment))
            {
                codeTypeDeclaration.Comments.Add(new CodeCommentStatement("<summary>", true));
                codeTypeDeclaration.Comments.Add(new CodeCommentStatement(comment, true));
                codeTypeDeclaration.Comments.Add(new CodeCommentStatement("</summary>", true));
            }
            
            codeNamespace.Types.Add(codeTypeDeclaration);
            codeCompileUnit.Namespaces.Add(codeNamespace);

            return codeCompileUnit;
        }

        public CodeMemberProperty CreateAutoProperty(Type type, string propertyName, string comment = "")
        {
            CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
            codeMemberProperty.Name = propertyName;
            codeMemberProperty.Type = new CodeTypeReference(type);
            codeMemberProperty.HasGet = true;
            codeMemberProperty.HasSet = true;
            if (! string.IsNullOrWhiteSpace(comment))
            {
                codeMemberProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                codeMemberProperty.Comments.Add(new CodeCommentStatement(comment, true));
                codeMemberProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
            }
            
            codeMemberProperty.Attributes = MemberAttributes.Public;

            return codeMemberProperty;
        }

        public CodeMemberProperty CreateAutoProperty(string typeName, string propertyName, string comment)
        {
            CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
            codeMemberProperty.Name = propertyName;
            codeMemberProperty.Type = new CodeTypeReference(typeName);
            codeMemberProperty.HasGet = true;
            codeMemberProperty.HasSet = true;
            if (!string.IsNullOrWhiteSpace(comment))
            {
                codeMemberProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
                codeMemberProperty.Comments.Add(new CodeCommentStatement(comment, true));
                codeMemberProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
            }
            codeMemberProperty.Attributes = MemberAttributes.Public;

            return codeMemberProperty;
        }
    }
}
