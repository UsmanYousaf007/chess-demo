using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;

namespace HUF.Utils.Runtime.PlayerPrefs
{
    public class StringArrayPP
    {
        readonly StringPP pref;
        readonly char separator;

        public StringArrayPP( string key, char separator )
        {
            pref = new StringPP( key );
            this.separator = separator;
        }

        /// <summary>
        /// Gets and sets the array.
        /// </summary>
        /// <param name="newElement"></param>
        [PublicAPI]
        public string[] Value
        {
            get { return pref.Value.IsNullOrEmpty() ? null : pref.Value.Split( separator ); }
            set
            {
                if ( value != null )
                    pref.Value = string.Join( separator.ToString(), value );
                else
                    Clear();
            }
        }

        /// <summary>
        /// Adds a new element to the array.
        /// </summary>
        /// <param name="newElement">A new element.</param>
        [PublicAPI]
        public void Add( string newElement )
        {
            pref.Value = pref.Value.IsNullOrEmpty() ? newElement : pref.Value + separator + newElement;
        }

        /// <summary>
        /// Adds a unique element to the array.
        /// </summary>
        /// <param name="newElement">A new element.</param>
        [PublicAPI]
        public void AddUnique( string newElement )
        {
            if ( !HasElement( newElement ) )
            {
                Add( newElement );
            }
        }

        /// <summary>
        /// Checks if the array contains an element.
        /// </summary>
        /// <param name="newElement">An element.</param>
        [PublicAPI]
        public bool HasElement( string element )
        {
            string[] values = Value;

            if ( values == null )
            {
                return false;
            }

            foreach ( var value in values )
            {
                if ( value == element )
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Deletes all array elements.
        /// </summary>
        [PublicAPI]
        public void Clear()
        {
            pref.Value = string.Empty;
        }
    }
}