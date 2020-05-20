using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HUF.Utils.Runtime.Attributes;
using HUF.Utils.Runtime.Configs.API;

#if UNITY_EDITOR

namespace HUF.Utils.Runtime.Configs.Implementation
{
    public static class ConfigPrevalidator
    {
        const string ERROR_MESSAGE = "Config is missing required properties";

        public static bool ValidateConfig( AbstractConfig config, out string message )
        {
            if ( !CheckRequiredFields( config ) )
            {
                message = ERROR_MESSAGE;
                return false;
            }

            message = string.Empty;
            return true;
        }

        static bool CheckRequiredFields( AbstractConfig config )
        {
            HashSet<FieldInfo> @checked = new HashSet<FieldInfo>();
            Type type = config.GetType();
            var fields = GatherFields( type, config ).ToList();

            bool EveluateValue( object o, RequiredAttribute requirement )
            {
                bool isNull = o == null || o.Equals( null );

                if ( isNull )
                    return false;

                if ( requirement.TargetType == null )
                    return !string.IsNullOrEmpty( o.ToString() );

                return requirement.TargetType.IsInstanceOfType( o );
            }

            int fieldCount = fields.Count;

            for ( int index = 0; index < fieldCount; index++ )
            {
                var field = fields[index];

                if ( @checked.Contains( field.field ) || typeof(AbstractConfig).IsAssignableFrom( field.type ) )
                    continue;

                @checked.Add( field.field );
                object fieldValue = field.field.GetValue( field.target );

                if ( field.field.IsDefined( typeof(RequiredAttribute) ) )
                {
                    if ( !EveluateValue( fieldValue, field.field.GetCustomAttribute<RequiredAttribute>() ) )
                        return false;
                }

                if ( fieldValue == null ||
                     fieldValue.Equals( null ) ||
                     field.type.IsPrimitive ||
                     field.type.IsSealed ||
                     !field.type.IsSerializable )
                    continue;

                var nestedFields = GatherFields( field.type, fieldValue );
                fields.AddRange( nestedFields );
                fieldCount = fields.Count;
            }

            return true;
        }

        static IEnumerable<FieldPair> GatherFields( Type type, object target )
        {
            var fields = type.GetFields(
                BindingFlags.Default |
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Instance );

            for ( int i = 0; i < fields.Length; i++ )
            {
                yield return new FieldPair( fields[i], target );
            }
        }

        struct FieldPair
        {
            public FieldInfo field;
            public object target;
            public Type type;

            public FieldPair( FieldInfo field, object target )
            {
                this.field = field;
                this.target = target;
                type = field.FieldType;
            }
        }
    }
}

#endif