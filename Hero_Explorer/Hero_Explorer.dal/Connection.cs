using Hero_Explorer.ent;
using Hero_Explorer.ent.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Hero_Explorer.dal
{
    public class Connection
    {
        private const string PrivateKey = "facd93f2276ac19f20f60afecba70fa15748d51f";
        private const string PublicKey = "46bba2979fe6e7254913c73b58202b8d";
        private const int MAXCAR = 1500;
        private const int MAXDESC = 20;
        //?nameStartsWith=s


        public static async Task<CharacterDataWrapper> getCharacterData()
        {
            Random random = new Random();
            var offset = random.Next(MAXCAR);

            string url = String.Format("https://gateway.marvel.com/v1/public/characters?limit={0}&offset={1}", MAXDESC, offset);

            var json = await CallMarvel(url);

            var serializer = new DataContractJsonSerializer(typeof(CharacterDataWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var result = (CharacterDataWrapper)serializer.ReadObject(ms);

            return result;
        }

        public static async Task<CharacterDataWrapper> getCharacterData(string str)
        {
            Random random = new Random();
            var offset = random.Next(MAXCAR);

            string url = String.Format("https://gateway.marvel.com/v1/public/characters?nameStartsWith={0}&limit={1}", str,MAXDESC);

            var json = await CallMarvel(url);

            var serializer = new DataContractJsonSerializer(typeof(CharacterDataWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var result = (CharacterDataWrapper)serializer.ReadObject(ms);

            return result;
        }

        public static async Task<ComicDataWrapper> getComicsData(int characterId)
        {
            var url = String.Format("http://gateway.marvel.com:80/v1/public/comics?characters={0}&limit=10",
                characterId);
            var json = await CallMarvel(url);

            var serializer = new DataContractJsonSerializer(typeof(ComicDataWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var result = (ComicDataWrapper)serializer.ReadObject(ms);

            return result;
        }

        #region Llamada a la Marvel

        private async static Task<string> CallMarvel(string url)
        {
            var timeSpam = DateTime.Now.Ticks.ToString();
            var hash = createHash(timeSpam);

            string completeUrl = String.Format("{0}&apikey={1}&ts={2}&hash={3}", url, PublicKey, timeSpam, hash);

            //Call to Marvel
            HttpClient http = new HttpClient();
            var response = await http.GetAsync(completeUrl);
            return await response.Content.ReadAsStringAsync();
        }

        private static object createHash(string timeStamp)
        {
            var toBeHashed = timeStamp + PrivateKey + PublicKey;
            var hashedMessage = ComputeMD5(toBeHashed);
            return hashedMessage;
        }

        private static object ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
        #endregion
    }
}
