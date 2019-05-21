using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace PlayStats.Services
{
    public interface IBggService
    {
        Task<IEnumerable<BggGameInfo>> SearchGames(string term, CancellationToken token);
        Task<BggGameDetail> LoadGameDetails(string id, CancellationToken token);
    }

    public class BggService : IBggService
    {
        public async Task<IEnumerable<BggGameInfo>> SearchGames(string term, CancellationToken token)
        {
            var htmlEncodedTerm = HttpUtility.HtmlEncode(term);
            var url = $"https://boardgamegeek.com/xmlapi/search?search={htmlEncodedTerm}";
            var httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync(new Uri(url), token);
            var xml = await httpResponse.Content.ReadAsByteArrayAsync();
            var doc = XDocument.Parse(Encoding.UTF8.GetString(xml));
            var boardGames = doc.Descendants("boardgame");

            var result = new List<BggGameInfo>();
            foreach (var boardGame in boardGames)
            {
                var name = HttpUtility.HtmlDecode(boardGame.Element("name").Value);
                var id = boardGame.Attribute("objectid").Value;
                result.Add(new BggGameInfo(name, id));
            }

            return result.AsEnumerable();
        }

        public async Task<BggGameDetail> LoadGameDetails(string id, CancellationToken token)
        {
            var url = $"https://boardgamegeek.com/xmlapi/boardgame/{id}";
            var httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync(new Uri(url), token);
            var xml = Encoding.UTF8.GetString(await httpResponse.Content.ReadAsByteArrayAsync());
            var doc = XDocument.Parse(xml);
            var boardGame = doc.Descendants("boardgame").FirstOrDefault();

            var result = new BggGameDetail();
            result.ObjectId = int.Parse(boardGame.Attribute("objectid").Value);
            result.FullName = boardGame.Element("name").Value;
            result.YearPublished = int.Parse(boardGame.Element("yearpublished").Value);
            result.Description = boardGame.Element("description").Value;
            result.Thumbnail = await (await httpClient.GetAsync(boardGame.Element("thumbnail").Value, token)).Content.ReadAsByteArrayAsync();
            result.Image = await (await httpClient.GetAsync(boardGame.Element("image").Value, token)).Content.ReadAsByteArrayAsync();
            result.Publishers = string.Join(", ", boardGame.Descendants("boardgamepublisher").Select(p => p.Value).ToList());
            result.Designers = string.Join(", ", boardGame.Descendants("boardgamedesigner").Select(p => p.Value).ToList());

            return result;
        }
    }
}
