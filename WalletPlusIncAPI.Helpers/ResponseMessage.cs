namespace WalletPlusIncAPI.Helpers
{
    /// <summary>
    ///
    /// </summary>
    public static class ResponseMessage
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="title"></param>
        /// <param name="errors"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object Message(string title, object errors = null, object data = null)
        {
            return new { title, errors, data };
        }
    }
}