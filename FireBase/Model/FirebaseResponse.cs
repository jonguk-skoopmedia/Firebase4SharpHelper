using System.Collections.Generic;
using System.Text;

namespace lvChartTest.Util.FireBase.Model
{
    public class FirebaseResponse
    {
        public bool Success { get; private set; }
        public int Code { get; private set; }
        public Dictionary<string, object> Body { get; private set; }
        public string RawBody { get; private set; }

        public FirebaseResponse(bool success, int code, Dictionary<string, object> body, string rawBody)
        {
            Success = success;
            Code = code;
            if(body == null)
            {
                // TODO: loging
                Body = new Dictionary<string, object>();
            }
            else
            {
                Body = body;
            }

            if(rawBody == null)
            {
                // TODO: loging
                rawBody = string.Empty;
            }
            else
            {
                RawBody = rawBody;
            }
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.Append(nameof(FirebaseResponse) + "[ " )
				.Append( "(Success:" ).Append(Success ).Append( ") " )
				.Append( "(Code:" ).Append(Code ).Append( ") " )
				.Append( "(Body:" ).Append(Body ).Append( ") " )
				.Append( "(Raw-body:" ).Append(RawBody ).Append( ") " )
				.Append( "]" );
		
		    return result.ToString();
        }
    }
}
