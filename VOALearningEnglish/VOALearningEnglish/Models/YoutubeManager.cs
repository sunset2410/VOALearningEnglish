using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VOALearningEnglish.Data;

namespace VOALearningEnglish.Models
{
    public class YoutubeManager
    {
        public YoutubeManager()
        {

        }

        //----------Youtub Data API Credentials---------------//
        YouTubeService youtubeService = new YouTubeService(
                new BaseClientService.Initializer
                {
                    ApiKey = "AIzaSyBpHmkpOxtY08WzXq",
                    ApplicationName = "YTube"
                });

        public async Task<List<YoutubeVideo>> GetListVideos(string NameChannel, CancellationToken token)
        {
            if (token.IsCancellationRequested) throw new OperationCanceledException();
            int max_results = 32;
            string userName = NameChannel;
            string YoutubeChannel_2 = await GetChannelId(userName);
            List<YoutubeVideo> channelVideos = await GetChannelVideos(YoutubeChannel_2, max_results, token);
            return channelVideos;
        }

        private async Task<List<YoutubeVideo>> GetChannelVideos(string channelId, int maxResults, CancellationToken token)
        {
            if (token.IsCancellationRequested) throw new OperationCanceledException();

            var channelItemsListRequest = youtubeService.Search.List("snippet");
            channelItemsListRequest.ChannelId = channelId;
            channelItemsListRequest.Type = "video";
            channelItemsListRequest.Order = Google.Apis.YouTube.v3.SearchResource.ListRequest.OrderEnum.Date;
            channelItemsListRequest.MaxResults = maxResults;
            channelItemsListRequest.PageToken = "";

            try
            {
                var channelItemsListResponse = await channelItemsListRequest.ExecuteAsync();
                List<YoutubeVideo> channelVideos = new List<YoutubeVideo>();

                if (token.IsCancellationRequested) throw new OperationCanceledException();

                foreach (var channelItem in channelItemsListResponse.Items)
                {
                    if (token.IsCancellationRequested) throw new OperationCanceledException();
                    channelVideos.Add(
                        new YoutubeVideo
                        {
                            Id = channelItem.Id.VideoId,
                            Title = channelItem.Snippet.Title,
                            Description = channelItem.Snippet.Description,
                            PubDate = channelItem.Snippet.PublishedAt,
                            Thumbnail = channelItem.Snippet.Thumbnails.Medium.Url,
                            YoutubeLink = "https://www.youtube.com/watch?v=" + channelItem.Id.VideoId,
                            Time = channelItem.Snippet.PublishedAt.ToString(),
                            IsLive = false
                        });
                }

                return channelVideos;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private async Task<string> GetChannelId(string userName)
        {
            var channelIdRequest = youtubeService.Channels.List("id");
            channelIdRequest.ForUsername = userName;
            var channelIdResponse = await channelIdRequest.ExecuteAsync();
            return channelIdResponse.Items.FirstOrDefault().Id;
        }


        public async Task<string> GetLinkLiveVideo(string urlinfo)
        {
            try
            {
                System.Net.Http.HttpClient myClient = new System.Net.Http.HttpClient();
                System.Net.Http.HttpResponseMessage responseGet = await myClient.GetAsync(urlinfo);
                string response = await responseGet.Content.ReadAsStringAsync();
                int begin, end;
                begin = response.IndexOf("hlsvp=") + 6 ;
                end = response.IndexOf("m3u8", begin) + 4;
                string Url = response.Substring(begin, end - begin);
                Url = Url.Replace("%3A", ":");
                Url = Url.Replace("%3a", ":");
                Url = Url.Replace("%2F", "/");
                Url = Url.Replace("%2f", "/");
                Url = Url.Replace("%252C", ",");
                Url = Url.Replace("%252c", ",");
                return Url;
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
           
        }

    }
}


// https://manifest.googlevideo.com/api/manifest/hls_variant/expire/1489782708/go/1/playlist_type/DVR/requiressl/yes/id/y60wDzZt8yg.5/itag/0/gcr/vn/signature/37D127107C506FE6DBAFEE404EE4DC801076AE28.8CA213FAE703C2D54B9DDDDF8028D66215206945/ipbits/0/sparams/gcr,go,id,ip,ipbits,itag,maudio,playlist_type,requiressl,source,expire/source/yt_live_broadcast/upn/-in0B3g73cg/maudio/1/key/yt6/ip/14.177.213.56/file/index.m3u8
// "https://manifest.googlevideo.com/api/manifest/hls_variant/id/gNosnzCaS4I.331/go/1/upn/bFN7uACJZSU/sparams/ei%2Cgcr%2Cgo%2Chfr%2Cid%2Cip%2Cipbits%2Citag%2Cmaudio%2Cplaylist_type%2Cratebypass%2Crequiressl%2Csource%2Cexpire/ei/0vvLWMihJsT04AK-r5S4BQ/hfr/1/requiressl/yes/signature/B63362E048ACD5521293976FC7044AA0EC0BE38F.0AF464B1E54F23C84517961D9718525AEA946AA1/itag/0/playlist_type/DVR/ipbits/0/ratebypass/yes/maudio/1/ip/14.177.213.56/key/yt6/beids/%5B9466593%5D/gcr/vn/dover/7/source/yt_live_broadcast/expire/1489784882/keepalive/yes/file/index.m3u8"
// "https://www.youtube.com/get_video_info?&video_id=gNosnzCaS4I"; dw
// https://www.youtube.com/get_video_info?&video_id=y60wDzZt8yg    sky
