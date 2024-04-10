using System.Text.Json;

namespace ObfuscateApp
{
    public static class StringDictionary
    {
        private static List<string> _words = new List<string>();
        public static async Task LoadWords()
        {
            string url = "https://random-word-api.herokuapp.com/word?number=1000";


            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string wordsJson = await response.Content.ReadAsStringAsync();
                    _words.AddRange(JsonSerializer.Deserialize<string[]>(wordsJson));
                }
                else
                {
                    throw new Exception("Не удалось получить случайное слово.");
                }
            }
        }

        public static string GetRandom()
        {
            Random random = new Random();
            return _words[random.Next(_words.Count)];
        }
    }
}
