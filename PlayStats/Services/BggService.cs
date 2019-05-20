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
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            var xml = await webClient.DownloadStringTaskAsync(new Uri(url));
            var doc = XDocument.Parse(xml);
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
            var webClient = new HttpClient();
            //webClient.Encoding = Encoding.UTF8;
            var xml = await webClient.GetAsync(new Uri(url), token);
            var doc = XDocument.Parse(Encoding.UTF8.GetString(await xml.Content.ReadAsByteArrayAsync()));
            var boardGame = doc.Descendants("boardgame").FirstOrDefault();

            var result = new BggGameDetail();
            result.ObjectId = int.Parse(boardGame.Attribute("objectid").Value);
            result.FullName = boardGame.Element("name").Value;
            result.YearPublished = int.Parse(boardGame.Element("yearpublished").Value);
            result.Description = boardGame.Element("description").Value;
            result.Thumbnail = await (await webClient.GetAsync(boardGame.Element("thumbnail").Value, token)).Content.ReadAsByteArrayAsync();
            result.Image = await (await webClient.GetAsync(boardGame.Element("image").Value, token)).Content.ReadAsByteArrayAsync();
            result.Publishers = boardGame.Descendants("boardgamepublisher").Select(p => new BggPublisher
                {ObjectId = int.Parse(p.Attribute("objectid").Value), Name = p.Value}).ToList();
            result.Designers = boardGame.Descendants("boardgamedesigner").Select(p => new BggDesigner
                {ObjectId = int.Parse(p.Attribute("objectid").Value), Name = p.Value}).ToList();
            
            return result;
        }
    }
}
