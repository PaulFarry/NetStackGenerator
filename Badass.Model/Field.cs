﻿using System;
using System.Linq;

namespace Badass.Model
{
    public class Field
    {
        private const int RankOffset = 1000000;

        public SimpleType Type { get; }

        public Field(SimpleType type)
        {
            Type = type;
        }

        public const string ModifiedFieldName = "modified";

        public const string CreatedFieldName = "created";

        public const string DeletedFieldName = "deleted";
        
        public const string SoftDeleteFieldName = "deleted_date";

        public const string SearchFieldName = "search_content";

        public const string ContentTypeFieldName = "content_type";

        public const string IdFieldName = "id";

        public const string ThumbnailFieldName = "thumbnail";

        public const string ColorFieldType = "color";

        public const string ThumbnailFieldType = "thumbnail";

        public string Name { get; set; }

        public int Order { get; set; }

        public dynamic Attributes { get; set; }

        public int? Size { get; set; }

        public bool IsRequired { get; set; }

        public ApplicationType ReferencesType { get; set; }
        public Field ReferencesTypeField { get; set; } // if this field is a foreign-key relationship to another type, this is the "column" for the foreign key.

        public bool IsIdentity { get; set; }

        public Type ClrType { get; set; }

        public string ProviderTypeName { get; set; }
        
        public bool IsGenerated { get; set; }
        
        public bool IsCallerProvided => !IsAutoAssignedIdentity && !IsTrackingDate && !IsDelete && !IsTrackingUser && !IsSearch && !IsExcludedFromResults && !IsGenerated;

        public bool IsUserEditable => IsCallerProvided && !IsAttachmentThumbnail && !IsAttachmentContentType;

        
        public bool IsTrackingDate => IsDateTime && (Name == CreatedFieldName || Name == ModifiedFieldName || Name == SoftDeleteFieldName);

        public bool IsTrackingUser => ((ReferencesType != null && ReferencesType.IsSecurityPrincipal) && (Name.StartsWith(CreatedFieldName) || Name.StartsWith(ModifiedFieldName) || (Type is ApplicationType && ((ApplicationType)Type).DeleteType == DeleteType.Soft && Name.StartsWith(DeletedFieldName))));

        public bool IsDelete => (ClrType == typeof(DateTime) || ClrType == typeof(DateTime?)) && Name == SoftDeleteFieldName;

        public bool IsSearch => ProviderTypeName == "tsvector" && Name == SearchFieldName;

        public bool IsExcludedFromResults => IsDelete || IsSearch;

        public bool HasReferenceType => ReferencesType != null;

        public bool IsDateTime => (ClrType == typeof(DateTime) || ClrType == typeof(DateTime?));

        public bool IsDate => Type.Domain.TypeProvider.IsDateOnly(ProviderTypeName);
        
        public bool IsBoolean => ClrType == typeof(bool) || ClrType == typeof(bool?);

        public bool IsFile => (this.ClrType == typeof(byte[]) || this.ClrType == typeof(byte?[]));

        public bool IsAutoAssignedIdentity
        {
            get
            {
                // TODO - this is a little crude
                return IsIdentity && (ClrType == typeof(int) || ClrType == typeof(Guid));
            }
        }
        
        public bool IsIntegerAssignedIdentity
        {
            get
            {
                return IsIdentity && ClrType == typeof(int);
            }
        }

        public bool IsAttachmentContentType
        {
            get
            {
                return Attributes?.isContentType == true || (UnderlyingType.IsAttachment && ClrType == typeof(string) &&
                                                             Name == ContentTypeFieldName);
            }
        }

        public bool IsAttachmentThumbnail
        {
            get
            {
                return Attributes?.type == ThumbnailFieldType || (UnderlyingType.IsAttachment && IsFile &&
                                                                  Name == ThumbnailFieldName);
            }
        }

        public bool IsAttachmentData
        {
            get
            {
                return IsFile && !IsAttachmentThumbnail;
            }
        }
        
        public bool IsLargeTextContent
        {
            get
            {
                return ClrType == typeof(string) && (Size > 500 || Attributes?.largeContent == true);
            }
        }
        
        public bool IsColor => Attributes?.type == ColorFieldType;

        public int Rank => Attributes?.rank != null ? (int)Attributes?.rank : RankOffset + Order;

        public bool IsRating => ((this.ClrType) == typeof(int) || (this.ClrType) == typeof(short)) &&
                                Attributes?.isRating == true;

        public bool Add => Attributes?.add != null ? Attributes.add : true;

        public bool Edit => Attributes?.edit != null ? Attributes.edit : !IsIdentity;

        public bool IsDisplayField => Attributes?.isDisplayForType != null ? Attributes.isDisplayForType : false;

        public bool IsInt => ClrType == typeof(int);

        public Field RelatedTypeField
        {
            get
            {
                if (Type is ResultType)
                {
                    return ((ResultType) Type).RelatedType?.Fields.FirstOrDefault(f => f.Name == Name);
                }

                if (Type is ApplicationType)
                {
                    return this;
                }
                
                return null;
            }
        }

        public ApplicationType UnderlyingType
        {
            get
            {
                if (Type is ApplicationType type)
                {
                    return type;
                }

                if (Type is ResultType resType)
                {
                    return resType.RelatedType;
                }

                return null;
            }
        }
    }
}