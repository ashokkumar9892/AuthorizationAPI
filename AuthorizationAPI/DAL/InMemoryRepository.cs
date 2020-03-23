using System.Collections.Generic;

namespace AuthorizationAPI.DAL
{
    public class InMemoryRepository
    {
        public static Dictionary<string, List<object>> repository = new Dictionary<string, List<object>>();

        
        public void Add<T>(T obj)
        {
            string key = typeof(T).Name;
            if (!repository.ContainsKey(key))
            {
                repository.Add(key, new List<object>());
            }
            repository[key].Add(obj);
        }

        public List<T> GetAll<T>()
        {
            string key = typeof(T).Name;
            var result = new List<T>();
            if (repository.ContainsKey(key))
            {
                foreach (var o in repository[key])
                {
                    result.Add((T)o);
                }
            }
            return  result;
        }


        internal void Remove<T>(T obj)
        {
            string key = typeof(T).Name;
            if (repository.ContainsKey(key))
            {
                repository[key].Remove(obj);
            }
        }
    }
}