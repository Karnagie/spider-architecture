namespace MVP.Model
{
    public static class Ids
    {
        private static int _ids;
        
        public static int GetNewId()
        {
            return _ids++;
        }
    }
}