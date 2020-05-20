using System.Text;

namespace HUF.Utils.Runtime.Logging {
    public class HLogPrefix {
        const string CHAIN_SEPARATOR = ":";

        readonly string localPrefix;
        readonly HLogPrefix parent;

        public static HLogPrefix Empty { get; } = new HLogPrefix( string.Empty );

        public string Prefix { get; }

        public HLogPrefix( string prefix ) {
            this.localPrefix = prefix;
            Prefix  = GeneratePrefix();
        }

        public HLogPrefix( HLogPrefix parent, string prefix ) {
            this.parent = parent;
            this.localPrefix = prefix;
            Prefix  = GeneratePrefix();
        }

        string GeneratePrefix() {
            StringBuilder builder = new StringBuilder();
            HLogPrefix    current = this.parent;

            while( current!=null ) {
                builder.Insert( 0, CHAIN_SEPARATOR );
                builder.Insert( 0, current.localPrefix );
                current = current.parent;
            }

            builder.Append( localPrefix );

            return builder.ToString();
        }

        public override string ToString()
        {
            return Prefix;
        }
    }
}