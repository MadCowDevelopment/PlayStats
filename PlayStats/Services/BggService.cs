using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    }

    public class BggGameInfo
    {
        public string Name { get; }
        public string Id { get; }

        public BggGameInfo(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public override string ToString()
        {
            return Name;
        }
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
    }
}
