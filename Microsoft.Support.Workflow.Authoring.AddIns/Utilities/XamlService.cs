using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;
using System.IO;
using System.Xml;
using System.Activities.XamlIntegration;
using System.Diagnostics.CodeAnalysis;
using System.Activities.Presentation;
using System.Activities.Presentation.Services;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using System.Diagnostics.Contracts;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    public static class XamlService
    {
        /// <summary>
        /// Replacement for WorkflowDesigner.Flush()/Text that can fully qualify XAML namespaces and thus support side-by-side versions of activity libraries in the authoring tool.
        /// Note that XamlBuildTask requires clr namespaces with no version/public key (inherently ambiguous) and should not use this method.
        /// </summary>
        /// <returns>XAML for workflow root, or empty string if no root</returns>
        public static string LooseXaml(this WorkflowDesigner workflowDesigner)
        {
            Contract.Requires(workflowDesigner != null);
            return GetXaml(workflowDesigner, fullyQualifiedClrNamespaces: false, shoudCleanup: false);
        }

        /// <summary>
        /// Serialize workflow root in a form suitable for XamlBuildTask. 
        /// </summary>
        /// <param name="workflowDesigner"></param>
        /// <returns></returns>
        public static string CompilableXaml(this WorkflowDesigner workflowDesigner)
        {
            Contract.Requires(workflowDesigner != null);
            return GetXaml(workflowDesigner, fullyQualifiedClrNamespaces: false, shoudCleanup: true);
        }

        static string GetXaml(WorkflowDesigner workflowDesigner, bool fullyQualifiedClrNamespaces, bool shoudCleanup)
        {
            Contract.Requires(workflowDesigner != null);
            var errorService = workflowDesigner.Context.IfNotNull(ctx => ctx.Services.GetService<IXamlLoadErrorService>() as Microsoft.Support.Workflow.Authoring.AddIns.ViewModels.WorkflowEditorViewModel.ErrorService);
            if (errorService.IfNotNull(err => err.HasXamlLoadErrors))
            {
                // When XamlLoad errors occur, designer is read-only. Not safe to reserialize because ErrorActivity will lose information
                // about the original Xaml, so we return the XAML originally loaded. (This is probably because the user directly edited
                // the XAML to be incorrect. We should preserve what he wrote so he can fix it.)
                return workflowDesigner.Text;
            }
            object root = workflowDesigner.Context.Services.GetService<ModelService>().IfNotNull(modelService => modelService.Root)
                .IfNotNull(v => v.GetCurrentValue());
            // Could be ActivityBuilder or Activity, or something else entirely actually
            if (root != null)
            {
                return shoudCleanup ? XamlService.SerializeToXamlWithCleanup(root, fullyQualifiedClrNamespaces: fullyQualifiedClrNamespaces) :
                    XamlService.SerializeToXaml(root, fullyQualifiedClrNamespaces: fullyQualifiedClrNamespaces);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// We should not rely on the default WorkflowDesigner.Text property because it doesn't emit fully-qualified XAML namespaces,
        /// which we need in order to make our side-by-side versioning story work in the authoring tool.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "XmlWriter.Dispose() will dispose of StringWriter too.")]
        public static string SerializeToXaml(object objectToSerialize, bool fullyQualifiedClrNamespaces = true)
        {
            var stringWriter = new StringWriter();
            var xamlSchemaContextCWithFullyQualifiedNameSupport =
                new XamlSchemaContext(new XamlSchemaContextSettings { FullyQualifyAssemblyNamesInClrNamespaces = fullyQualifiedClrNamespaces });
            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true }))
            using (var xamlWriter = new XamlXmlWriter(xmlWriter, xamlSchemaContextCWithFullyQualifiedNameSupport))
            using (var builderWriter = ActivityXamlServices.CreateBuilderWriter(xamlWriter)) // transform to x:Class representation
            using (var objectReader = new XamlObjectReader(objectToSerialize, xamlSchemaContextCWithFullyQualifiedNameSupport))
            {
                XamlServices.Transform(objectReader, builderWriter);
                return stringWriter.ToString();
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "XmlWriter.Dispose() will dispose of StringWriter too.")]
        public static string SerializeToXamlWithCleanup(object objectToSerialize, bool fullyQualifiedClrNamespaces = true)
        {
            var stringWriter = new StringWriter();
            var xamlSchemaContextCWithFullyQualifiedNameSupport =
                new XamlSchemaContext(new XamlSchemaContextSettings { FullyQualifyAssemblyNamesInClrNamespaces = fullyQualifiedClrNamespaces });
            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true }))
            using (var xamlWriter = new XamlXmlWriter(xmlWriter, xamlSchemaContextCWithFullyQualifiedNameSupport))
            using (var builderWriter = ActivityXamlServices.CreateBuilderWriter(xamlWriter)) // transform to x:Class representation
            using (var objectReader = new XamlObjectReader(objectToSerialize, xamlSchemaContextCWithFullyQualifiedNameSupport))
            using (var cleanupReader = new SAPCleanupReader(objectReader))
            {
                XamlServices.Transform(cleanupReader, builderWriter);
                return stringWriter.ToString();
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "XmlWriter.Dispose() will dispose of StringWriter too.")]
        public static object DeserializeString(string text)
        {
            var xamlSchemaContextCWithFullyQualifiedNameSupport =
              new XamlSchemaContext(new XamlSchemaContextSettings { FullyQualifyAssemblyNamesInClrNamespaces = false });

            using (XamlXmlReader xamlXmlReader =
                new XamlXmlReader(XmlReader.Create(new StringReader(text)),
                        xamlSchemaContextCWithFullyQualifiedNameSupport,
                        new XamlXmlReaderSettings { ProvideLineInfo = true }))
            {
                using (System.Xaml.XamlReader activityBuilderReader = ActivityXamlServices.CreateBuilderReader(xamlXmlReader, xamlSchemaContextCWithFullyQualifiedNameSupport))
                {
                    XamlObjectWriter objectWriter = new XamlObjectWriter(activityBuilderReader.SchemaContext);
                    XamlServices.Transform(activityBuilderReader, objectWriter);
                    return objectWriter.Result;
                }
            }
        }

        /// <summary>
        /// Remove clutter like ViewState from XAML to improve readability and runtime perf (because we won't load System.Activities.Presentation)
        /// </summary>
        class SAPCleanupReader : XamlReader
        {
            public static readonly string[] ignoreNamespace = new[] 
            {
                // Remove sap clutter
                "http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation",
                "clr-namespace:Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;assembly=Microsoft.Support.Workflow.Authoring.AddIns",
            };

            XamlReader inner;
            public SAPCleanupReader(XamlReader inner)
            {
                this.inner = inner;
            }

            public override bool IsEof
            {
                get { return inner.IsEof; }
            }

            public override XamlMember Member
            {
                get { return inner.Member; }
            }

            public override NamespaceDeclaration Namespace
            {
                get { return inner.Namespace; }
            }

            public override XamlNodeType NodeType
            {
                get { return inner.NodeType; }
            }

            public override bool Read()
            {
                // Read next token
                inner.Read();
                // If the current token comes from a namespace we're trying to ignore,
                // skip that whole XAML subtree. Repeat until we find one we can't ignore,
                // or we run out of XAML.
                while (true)
                {
                    if (inner.NodeType == XamlNodeType.NamespaceDeclaration)
                    {
                        if (ignoreNamespace.Contains(inner.Namespace.Namespace))
                        {
                            inner.Skip(); // skip namespace declaration, e.g. xmlns:sap="http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation"
                            continue;
                        }
                    }
                    if (inner.NodeType == XamlNodeType.StartMember)
                    {
                        if (ignoreNamespace.Contains(inner.Member.PreferredXamlNamespace))
                        {
                            inner.Skip(); // skip member and everything inside, e.g. <sap:WorkflowViewStateService.ViewState>...</sap:WorkflowViewStateService.ViewState>
                            continue;
                        }
                    }
                    return !inner.IsEof;
                }
            }

            public override XamlSchemaContext SchemaContext
            {
                get { return inner.SchemaContext; }
            }

            public override XamlType Type
            {
                get { return inner.Type; }
            }

            public override object Value
            {
                get { return inner.Value; }
            }
        }
    }
}
