using _1999.Serialization;
using _1999.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Configuration;
using System.Web;
using System.IO;
using _1999.Models;
using System.Diagnostics;
//using Microsoft.Cognitive.LUIS;

namespace _1999.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity message)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));

            //string resposta = "Test Message-好";
            var resposta = await Response(message); //跟LUIS取得辩試結果，並準備回覆訊息
            if (resposta != null)
            {
                /////////////計算字數/////////////////////////
                // calculate something for us to return
                int length = (message.Text ?? string.Empty).Length;

                // return our reply to the user
                //Activity msg = message.CreateReply($"You sent {message.Text} which was {length} characters", "zh-TW");
                //await connector.Conversations.ReplyToActivityAsync(msg);

                //////////////回傳LIUS結果/////////////////////
                var msg = message.CreateReply(resposta , "zh-TW");
                ///////////////測試字數////////////////////////
                //var msg = message.CreateReply(resposta + $"You sent {message.Text} which was {length} characters", "zh-TW");

                await connector.Conversations.ReplyToActivityAsync(msg); //回傳訊息
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted); //回傳狀態

        }
        private static async Task<Activity> GetLuisResponse(Activity message)
        {
            Activity resposta = new Activity();

            var luisResponse = await Luis.GetResponse(message.Text); //Call LUIS Service
            if (luisResponse != null)
            {
                var intent = new Intent();
                var entity = new Serialization.Entity();

                //string acao = string.Empty;
                string road = string.Empty; //道路不平
                string streetlight = string.Empty; //路燈故障
                string dirty_noise_air = string.Empty; //髒亂、噪音、空汙
                string puddle = string.Empty; //積水、汙水管

                string entityType = string.Empty;

                int replaceStartPos = 0;
                resposta = message.CreateReply("我不識字XD");

                foreach (var item in luisResponse.entities)
                {
                    entityType = item.type;
                    replaceStartPos = entityType.IndexOf("::");
                    if (replaceStartPos > 0)
                        entityType = entityType.Substring(0, replaceStartPos);

                    switch (entityType)
                    {
                        case "道路不平":
                            road = item.entity;
                            break;
                        case "路燈故障":
                            streetlight = item.entity;
                            break;
                        case "髒亂、噪音、空汙":
                            dirty_noise_air = item.entity;
                            break;
                        case "積水、汙水管":
                            puddle = item.entity;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(road))
                {
                    resposta = message.CreateReply($"您說的報修問題是〔{road}〕，報修分類為「" + entityType + "」");
                }
                if (!string.IsNullOrEmpty(streetlight))
                {
                    resposta = message.CreateReply($"您說的報修問題是〔{streetlight}〕，報修分類為「" + entityType + "」");
                }
                if (!string.IsNullOrEmpty(dirty_noise_air))
                {
                    resposta = message.CreateReply($"您說的報修問題是〔{dirty_noise_air}〕，報修分類為「" + entityType + "」");
                }
                if (!string.IsNullOrEmpty(puddle))
                {
                    resposta = message.CreateReply($"您說的報修問題是〔{puddle}〕，報修分類為「" + entityType + "」");
                }
                else
                { 
                    //resposta = message.CreateReply("我聽不懂2");
                    resposta = message.CreateReply("報修分類為「" + entityType + "」"); //，可以再請您描述詳細一點需要報修的問題嗎？
                }
            }
            return resposta;
        }
        private static async Task<string> Response(Activity message)
        {
            Activity resposta = null; //回覆給客戶訊息
            if (message != null)
            {
                switch (message.GetActivityType())
                {
                    case ActivityTypes.Message:
                        resposta = await GetLuisResponse(message);
                        break;
                    case ActivityTypes.ConversationUpdate:
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    default:
                        //Trace.TraceError($"Unknown activity type ignored: {message.GetActivityType()}");
                        break;

                }
            }

            if (resposta != null)
                return resposta.Text;
            else
                return null;
        }
    }
}