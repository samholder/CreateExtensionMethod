using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.Util;

namespace CreateExtensionMethod
{
    using System;
    using System.Linq;
    using JetBrains.Application.Progress;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Daemon.CSharp.Errors;
    using JetBrains.ReSharper.Feature.Services.QuickFixes;
    using JetBrains.ReSharper.Psi.CSharp;
    using JetBrains.ReSharper.Psi.CSharp.Tree;
    using JetBrains.ReSharper.Psi.Modules;
    using JetBrains.ReSharper.Psi.Resolve;
    using JetBrains.ReSharper.Psi.Util;
    using JetBrains.TextControl;

    [QuickFix]
    public sealed class CreateExtensionMethod : QuickFixBase
    {
        private readonly NotResolvedError error;

        public CreateExtensionMethod(NotResolvedError error)
        {
            this.error = error;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var treeNode = error.Reference.GetTreeNode();
            var unresolvedThing = treeNode as IReferenceExpression;
            if (unresolvedThing == null)
            {
                return null;
            }
            var unresolvedThingsQualifier = unresolvedThing.QualifierExpression as IReferenceExpression;
            if (unresolvedThingsQualifier == null)
            {
                return null;
            }
            
            var qualifierElement = unresolvedThingsQualifier.Reference.Resolve().DeclaredElement;
            var qualifierElementType = qualifierElement.Type();
            if (qualifierElementType == null)
            {
                return null;
            }
            var classDeclaration = unresolvedThing.GetContainingTypeDeclaration();
            var nameSpaceDeclaration = unresolvedThing.GetContainingNamespaceDeclaration();
            if (classDeclaration == null || nameSpaceDeclaration == null)
            {
                return null;
            }
            var factory = CSharpElementFactory.GetInstance(treeNode.GetPsiModule());
            var presentableName = qualifierElementType.GetPresentableName(CSharpLanguage.Instance);
            var newClass = (IClassDeclaration)factory.CreateTypeMemberDeclaration("public static class $0Extensions{}",presentableName);
            var newMethod = (IClassMemberDeclaration)factory.CreateTypeMemberDeclaration("public static void $0 (this $1 $2){}", error.Reference.GetName(), presentableName, WithFirstLetterLowerCase(presentableName));
            newClass.AddClassMemberDeclaration(newMethod);
            
            nameSpaceDeclaration.AddTypeDeclarationAfter(newClass, classDeclaration);
            
            return null;
        }

        private static string WithFirstLetterLowerCase(string presentableName)
        {
            return presentableName.Substring(0, 1).ToLower() + presentableName.Substring(1, presentableName.Length - 1);
        }

        public override string Text
        {
            get
            {
                return "Create extension method";
            }
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return true;
        }
    }
}
