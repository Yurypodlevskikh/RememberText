namespace RememberText.Infrastructure.Helpers
{
    public class JsonResponseFactory
    {
        public static object ErrorResponse()
        {
            return new { success = false };
        }

        public static object ErrorResponseMessage(string error)
        {
            return new { success = false, message = error };
        }
        public static object ErrorResponseObject(object referenceObject)
        {
            return new { success = false, respobj = referenceObject };
        }

        public static object SuccessResponse()
        {
            return new { success = true };
        }

        public static object SuccessResponseMessage(string success)
        {
            return new { success = true, message = success };
        }

        public static object SuccessResponseObject(object referenceObject)
        {
            return new { success = true, respobj = referenceObject };
        }
    }
}
