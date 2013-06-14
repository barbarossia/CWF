// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentManager.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Support.Workflow.Authoring.Common.Messages;

namespace Microsoft.Support.Workflow.Authoring.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using Models;


    /// <summary>
    /// The content manager.
    /// </summary>
    public class ContentManager
    {
       /// <summary>
        /// The get content file items.
        /// </summary>
        /// <returns>
        /// ContentFileItem collection.
        /// </returns>
        public static ObservableCollection<ContentFileItem> GetContentFileItems()
        {
            string contentDirPath = Utility.GetContentDirectoryPath();
            var contentFileItems = new ConcurrentBag<ContentFileItem>();

            try
            {
                if (!String.IsNullOrEmpty(contentDirPath) && File.Exists(contentDirPath))
                {
                    XDocument contentDirectory = XDocument.Load(contentDirPath);
                    IEnumerable<XElement> files = contentDirectory.Descendants("ContentFile");

                    Parallel.ForEach(files, currentFile =>
                    {
                        var schemas = new XmlSchemaSet();
                        bool validationErrors;


                        XElement schemaElement = currentFile.Element("SchemaPath");
                        if ((schemaElement != null) && File.Exists(schemaElement.Value))
                        {
                            using (var reader = XmlReader.Create(schemaElement.Value))
                            {
                                schemas.Add("", reader);
                            }
                        }

                        XElement contentElement = currentFile.Element("FilePath");

                        if (contentElement != null)
                        {
                            if (File.Exists(contentElement.Value))
                            {
                                try
                                {
                                    var contentFileItem = new ContentFileItem
                                                              {
                                                                  FileName = contentElement.Value,
                                                                  FileShortName = Path.GetFileName(contentElement.Value),
                                                                  Content = XDocument.Load(contentElement.Value)
                                                              };

                                    validationErrors = false;
                                    contentFileItem.Content.Validate(schemas, (o, e) =>
                                    {
                                        validationErrors = true;
                                    });

                                    if (!validationErrors)
                                    {
                                        contentFileItems.Add(contentFileItem);
                                    }
                                }
                                catch (XmlException)
                                {
                                    MessageBoxService.ShowError(string.Format(CommonMessages.InvalidContentFile, Path.GetFileName(contentElement.Value)));
                                }
                                
                            }
                        }
                    }
                    );
                }
            }
            catch (XmlException)
            {
                //Content Directory File not valid or well-formed
                //TODO: Decide if user should be notified and how
            }


            return new ObservableCollection<ContentFileItem>(contentFileItems);
        }

        /// <summary>
        /// The get content items.
        /// </summary>
        /// <param name="contentFileItems">
        /// The content File Items.
        /// </param>
        /// <returns>
        /// ContentItems contained in XML files.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static ObservableCollection<ContentItem> GetContentItems(IEnumerable<ContentFileItem> contentFileItems)
        {
            ConcurrentBag<ContentItem> contentItemList = new ConcurrentBag<ContentItem>();

            Parallel.ForEach(contentFileItems, contentFileItem =>
                {
                    List<string> validNodeNames = new List<string>(new string[] { "key", "vartype", "collection" });

                    var sections = from items in contentFileItem.Content.Descendants()
                                   where items.Name.LocalName == "section"
                                   select items;

                    Parallel.ForEach(sections, rootItem =>
                    {
                        Parallel.ForEach(rootItem.Elements(), item =>
                        {
                            if (validNodeNames.Contains(item.Name.LocalName.ToLower()))
                            {
                                var ci = new ContentItem
                                             {
                                                 ContentFileName = contentFileItem.FileName,
                                                 Key = item.Attribute("name").Value
                                             };

                                if (item.Attribute("value") != null)
                                {
                                    ci.Value = item.Attribute("value").Value;
                                }
                                else if (item.Attribute("Description") != null)
                                {
                                    ci.Value = item.Attribute("Description").Value;
                                }
                                else
                                {
                                    //Build the list hint with some real data of the children nodes.
                                    if (item.HasElements)
                                    {
                                        var builder = new StringBuilder();
                                        builder.Append("List (");
                                        foreach (XElement element in item.Elements())
                                        {
                                            if (element.Attribute("name") != null)
                                            {
                                                builder.Append(element.Attribute("name").Value);
                                                builder.Append(",");
                                            }

                                            //We only need to show a small portion of data...
                                            if (builder.Length > 25)
                                            {
                                                break;
                                            }
                                        }

                                        builder.Append("...)");

                                        ci.Value = builder.ToString();
                                        builder.Clear();
                                    }
                                }

                                //Add t he section so the collection can be grouped later
                                ci.Section = contentFileItem.FileShortName;
                                contentItemList.Add(ci);
                            }
                        });
                    });
                });

            return new ObservableCollection<ContentItem>(contentItemList.OrderBy(e=>e.Key));
        }

       
    }
}