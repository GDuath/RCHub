using Microsoft.AspNetCore.Routing.Constraints;
using SkiaSharp;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using RCHub.Models.Dynmap;

namespace RCHub.Services
{

    public class DynmapService
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly HttpClient _httpClient;

        private DynmapContent _dynmapContent = new();
        private DateTime _lastCheck = DateTime.MinValue;


        const float _worldWidth = 73728f;
        const float _worldHeight = 36864f;



        public DynmapService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://map.rulercraft.com/")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task UpdateDynmapContent()
        {
            if (DateTime.Now.Subtract(_lastCheck).TotalMinutes >= 5d)
            {
                HttpResponseMessage response = await _httpClient.GetAsync("tiles/_markers_/marker_RulerEarth.json");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _dynmapContent = await response.Content.ReadFromJsonAsync<DynmapContent>() ?? throw new Exception("Could not parse dynmap content!");
                }

                _lastCheck = DateTime.Now;
            }
        }

        public async Task GenerateNationMap(string nationName)
        {
            await UpdateDynmapContent();

            var towns = _dynmapContent.Sets["towny.markerset"].Areas.Where(a => a.Value.Desc.Contains($"&#x1f38c; {nationName}")).ToList();
            var townMarkers = _dynmapContent.Sets["towny.markerset"].Markers.Where(a => a.Value.Desc.Contains($"&#x1f38c; {nationName}")).ToList();

            string backgroundImagePath = Path.Combine(_hostEnvironment.WebRootPath, "img", "world-map-topo-bw.png");

            using (var backgroundImageStream = System.IO.File.OpenRead(backgroundImagePath))
            {
                SKBitmap image = SKBitmap.Decode(backgroundImageStream);

                var dark = true;
                float colorsum = 0f;
                var rnd = new Random();
                for (int i = 0; i < 10; i++)
                {
                    var pixel = image.GetPixel(rnd.Next(0, image.Width), rnd.Next(0, image.Height));
                    colorsum += ((float)pixel.Red + (float)pixel.Green + (float)pixel.Blue) / 3f;
                }
                float average = colorsum / 10f;

                if (average >= 128)
                    dark = false;

                float scale = (float)image.Width / (float)_worldWidth;
                float offsetX = 36864f * scale;
                float offsetY = 18432f * scale;

                using (SKFontManager fontManager = SKFontManager.CreateDefault())
                using (SKCanvas canvas = new SKCanvas(image))
                {
                    SKPaint paint = new SKPaint()
                    {
                        Style = SKPaintStyle.Stroke,
                        Color = SKColor.Parse("#FFAA0000"),
                        StrokeWidth = 1,
                        Typeface = fontManager.CreateTypeface(_hostEnvironment.WebRootPath + "/fonts/RobotoCondensed-Regular.ttf")
                    };

                    foreach (var kvp in towns)
                    {
                        if (kvp.Key.Contains("Treasure_Ship") || kvp.Key.Contains("Quarry"))
                            continue;

                        var town = _dynmapContent.Sets["towny.markerset"].Areas[kvp.Key];

                        SKPath path = new SKPath();

                        SKPoint[] points = new SKPoint[town.X.Count];
                        for (int i = 0; i < town.X.Count; i++)
                            points[i] = new SKPoint((float)town.X[i] * scale + offsetX, (float)town.Z[i] * scale + offsetY);

                        path.AddPoly(points, true);

                        paint.Style = SKPaintStyle.Fill;
                        paint.Color = SKColor.Parse("#66" + town.Fillcolor.Replace("#", ""));

                        canvas.DrawPath(path, paint);

                        paint.Style = SKPaintStyle.Stroke;
                        paint.Color = SKColor.Parse(town.Color);

                        canvas.DrawPath(path, paint);

                    }

                    foreach (var kvp in townMarkers)
                    {
                        if (kvp.Key.Contains("Treasure_Ship") || kvp.Key.Contains("Quarry"))
                            continue;

                        var marker = _dynmapContent.Sets["towny.markerset"].Markers[kvp.Key];

                        paint.Style = SKPaintStyle.Fill;
                        paint.Color = dark ? SKColor.Parse("#FFFFFFFF") : SKColor.Parse("#FF000000");
                        paint.TextSize = 12f;
                        paint.IsAntialias = true;

                        canvas.DrawText(marker.Label.Replace('_', ' '), new SKPoint((float)marker.X * scale + offsetX + 3f, (float)marker.Z * scale + offsetY - 3f), paint);

                        paint.Style = SKPaintStyle.Stroke;
                        paint.Color = dark ? SKColor.Parse("#FFFFFFFF") : SKColor.Parse("#FF000000");
                        paint.StrokeWidth = 4;
                        canvas.DrawPoint(new SKPoint((float)marker.X * scale + offsetX, (float)marker.Z * scale + offsetY), paint);


                        if (marker.Icon == "ruler")
                        {
                            paint.Style = SKPaintStyle.Fill;
                            paint.Color = SKColor.Parse("#11FF0000");

                            canvas.DrawCircle(new SKPoint((float)marker.X * scale + offsetX, (float)marker.Z * scale + offsetY), 12200 * scale, paint);

                            paint.Style = SKPaintStyle.Stroke;
                            paint.Color = SKColor.Parse("#22FF0000");
                            paint.StrokeWidth = 2;

                            canvas.DrawCircle(new SKPoint((float)marker.X * scale + offsetX, (float)marker.Z * scale + offsetY), 12200 * scale, paint);

                        }

                        //string pattern = $"{Regex.Escape("Residents:</span>")}(.*?){Regex.Escape("<br /></p>")}";
                        //Match match = Regex.Match(marker.Desc, pattern);

                        //if (match.Success)
                        //{
                        //    TownInfos.Add(new TownInfo() { Name = marker.Label.Replace('_', ' '), Residents = int.Parse(match.Groups[1].Value.Trim()) });
                        //}


                    }

                    canvas.Save();
                }

                var filename = nationName + ".png";
                var outputPath = Path.Combine(_hostEnvironment.WebRootPath, "Data/Maps");
                if (!Directory.Exists(outputPath)) 
                    Directory.CreateDirectory(outputPath);

                var outputImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Data/Maps", filename);

                using (var outputImageStream = new FileStream(outputImagePath, FileMode.Create))
                {
                    image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(outputImageStream);
                }
            }



        }

        public async Task<List<string>> GetDynmapNations()
        {
            await UpdateDynmapContent();

            var result = new List<string>();

            foreach (var kvp in _dynmapContent.Sets["towny.markerset"].Markers)
            {
                var marker = kvp.Value;

                string pattern = $"{Regex.Escape("&#x1f38c;")}(.*?){Regex.Escape("</span>")}";
                Match match = Regex.Match(marker.Desc, pattern);
                if (match.Success)
                {
                    var name = match.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
                        result.Add(name);
                }
            }

            return result.Distinct().OrderBy(o => o).ToList();
        }

        public async Task<Dictionary<string, DynmapTownInfo>> GetNationTowns(string nationName)
        {
            await UpdateDynmapContent();

            var result = new Dictionary<string, DynmapTownInfo>();

            var townMarkers = _dynmapContent.Sets["towny.markerset"].Markers.Where(a => a.Value.Desc.Contains($"&#x1f38c; {nationName}")).ToList();

            foreach (var kvp in townMarkers)
            {
                if (kvp.Key.Contains("Treasure_Ship") || kvp.Key.Contains("Quarry"))
                    continue;

                var marker = _dynmapContent.Sets["towny.markerset"].Markers[kvp.Key];

                DynmapTownInfo townInfo = new DynmapTownInfo();
                townInfo.Name = marker.Label.Replace('_', ' ');

                string pattern = $"{Regex.Escape("Residents:</span>")}(.*?){Regex.Escape("<br /></p>")}";
                Match match = Regex.Match(marker.Desc, pattern);
                if (match.Success)
                    townInfo.Residents = int.Parse(match.Groups[1].Value.Trim());

                pattern = $"{Regex.Escape("&#x1f451; Ruler:</span></p> <span style=\"font-size:100%\">")}(.*?){Regex.Escape("</span>")}";
                match = Regex.Match(marker.Desc, pattern);
                if (match.Success)
                    townInfo.Mayor = match.Groups[1].Value.Trim();

                if (!result.ContainsKey(townInfo.Name)) 
                    result.Add(townInfo.Name, townInfo);
            }

            return result;
        }

        public async Task<DynmapTownInfo?> GetPlayerTown(string playerName)
        {
            await UpdateDynmapContent();

            KeyValuePair<string, Marker>? marker = _dynmapContent.Sets["towny.markerset"].Markers.FirstOrDefault(a => a.Value.Desc.Contains(playerName, StringComparison.OrdinalIgnoreCase));

            if (marker != null)
            {
                DynmapTownInfo? townInfo = new DynmapTownInfo
                {
                    Name = marker.Value.Value.Label
                };
                return townInfo;
            }

            return null;
        }
    }
}
