namespace Infrastructure.Services.Ids
{
    public static class IdProvider
    {
        private static int _ids;
        
        public static int GetNewId()
        {
            return _ids++;
        }
    }
}