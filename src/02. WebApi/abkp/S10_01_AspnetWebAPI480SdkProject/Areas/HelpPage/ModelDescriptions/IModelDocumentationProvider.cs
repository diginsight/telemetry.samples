using System;
using System.Reflection;

namespace S10_01_AspnetWebAPI480SdkProject.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}