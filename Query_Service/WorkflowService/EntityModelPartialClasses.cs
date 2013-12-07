using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Support.Workflow;

namespace Microsoft.Support.Workflow.Catalog
{


    public partial class Context
    {
        public xxContextItem ConvertToContextItem()
        {
            //Get a list of categories here because timing of entity
            //items does not allow conversion.

            xxContextItem anItem = new xxContextItem();

            anItem.Id = this.Id;
            anItem.Name = this.Name;
            anItem.ValueType = this.ValueType;
            anItem.IsValidated = this.IsValidated;
            anItem.IsPicklist = this.IsPickList;
            anItem.ValidationExecutable = this.ValidationExecuteable;
            anItem.ValidationMethod = this.ValidationMethod;
            anItem.Category = this.ContextCategory.ConvertToContextCategory();
            anItem.ShortName = this.ShortName;
            anItem.Description = this.Description;
            anItem.SecureData = (bool)this.SecureData;
            anItem.PrivacyData = (bool)this.PrivacyData;
            anItem.NotInDesign = (bool)this.NotInDesign;
            anItem.Persistable = (bool)this.Persistable;
            anItem.NotInDb = (bool)this.NotInDB;
            anItem.Tags = this.Tags;

            return anItem;
        }
        public static Context ConvertFromContextItem(xxContextItem contextItem)
        {
            return new Context()
            {
                Id = contextItem.Id,
                Name = contextItem.Name,
                ValueType = contextItem.ValueType,
                IsValidated = contextItem.IsValidated,
                IsPickList = contextItem.IsPicklist,
                ValidationExecuteable = contextItem.ValidationExecutable,
                ValidationMethod = contextItem.ValidationMethod,
                Category = contextItem.Category.Id,
                ShortName = contextItem.ShortName,
                Description = contextItem.Description,
                SecureData = contextItem.SecureData,
                PrivacyData = contextItem.PrivacyData,
                NotInDesign = contextItem.NotInDesign,
                Persistable = contextItem.Persistable,
                NotInDB = contextItem.NotInDb,
                Tags = contextItem.Tags
            };
        }
    }

    public partial class ContextCategory
    {
        public Microsoft.Support.Workflow.xxContextCategory ConvertToContextCategory()
        {

            return new Support.Workflow.xxContextCategory()
            {

                Id = this.Id,
                Name = this.Name,
                Description = this.Description
            };

            
        }

        public static ContextCategory ConvertFromContextCategory(Microsoft.Support.Workflow.xxContextCategory aCategory)
        {
            return new ContextCategory() 
            { 
                Id = aCategory.Id, 
                Name = aCategory.Name, 
                Description = aCategory.Description};
        }
    }
}