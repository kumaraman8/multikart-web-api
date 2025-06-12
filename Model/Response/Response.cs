namespace E_CommerceNet.Model.Response
{

    #region CommonResponse
    public class CommonResponse
    {
        public int resCode { get; set; }
        public object? resData { get; set; }
        public string? resMessage { get; set; }
    }
    public static class CommonResponseStatus
    {
        public const int
            SUCCESS = 200,
            BAD_REPUEST = 400,
            UNAUTHORIZED = 401,
            BLOCK = 403,
            NOT_FOUND = 404,
            DUPLICATE_FOUND = 409,
            INTERNAL_SERVER_ERROR = 500;
    }
    public static class CommonResponseMessage
    {
        public const string
            SUCCESS = "success",
            BAD_REQUEST = "failed",
            UNAUTHORIZED = "Unauthorized or session expired !!",
            BLOCK = "Service suspended. Please contact to admin !!",
            NOT_FOUND = "Data not found !!",
            DUPLICATE_FOUND = "Duplicate record found !!",
            INTERNAL_SERVER_ERROR = "Internal server error !!";
    }
    #endregion

}
