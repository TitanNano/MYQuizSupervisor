﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Collections.ObjectModel;

namespace MYQuizSupervisor
{
    //Networking als Singleton
    public sealed class Networking
    {

        private static Networking instance = null;
        //für Thread Safety - padlock
        private static readonly object padlock = new object();

        string hostAddress = string.Empty;

        const string contentType = "application/json";
        const bool useFakeData = false;

        private ObservableCollection<Group> CachedGroupList;
        private long LastGroupListRefresh = 0;

        public Networking(string hostAddress)
        {

            this.hostAddress = hostAddress;

        }

        //Eine Instanz erstellen bzw. wenn erstellt diese zurückgeben mit Thread-Safety
        public static Networking Current
        {
            get
            {
                lock (padlock)
                {
                    if(instance == null)
                    {
                        instance = new Networking("http://h2653223.stratoserver.net");
                    }
                    return instance;
                }
            }
        }

        public long getUnixTimestamp()
        {
            long unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            return unixTimestamp;
        }

        private async Task<T> sendRequest<T>(string route, string methode, object postData)
        {

            if (useFakeData)
            {
                //Fake data for offline service
                return default(T);
            }

            HttpWebRequest request = WebRequest.CreateHttp(hostAddress + route);

            request.ContentType = contentType;
            request.Method = methode;

            if (methode != "GET")
            {
                //Json soll Parameter mit NULL beim String-erstellen ignorieren
                var jsonString = JsonConvert.SerializeObject(postData,
                                                        Newtonsoft.Json.Formatting.None,
                                                        new JsonSerializerSettings
                                                        {
                                                            NullValueHandling = NullValueHandling.Ignore
                                                        });
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
                Stream dataStream = await request.GetRequestStreamAsync();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Dispose();

            }

            WebResponse response = await request.GetResponseAsync();

            StreamReader reader = new StreamReader(response.GetResponseStream());

            T value = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());

            response.Dispose();
            reader.Dispose();

            return value;

        }

        

        //ClientDevice registrieren        
        public async Task<RegistrationDevice> registerClientDevice(string token = null, string password = null)
        {
            var postData = new RegistrationDevice()
            {
                token = token,
                password = password,
            };

            RegistrationDevice device = await sendRequest<RegistrationDevice>("/api/devices", "POST", postData);          

            return device;
        }
        
                 
  

        //Client sendet beantwortete Frage
        public async void sendAnsweredQuestion(string groupId, string questionId, GivenAnswer answer)
        {
            string route = "/api/groups/" + groupId + "/questions/" + questionId + "/answers";

            var postData = JsonConvert.SerializeObject(answer);

            var result = await sendRequest<object>(route, "POST", postData);
        }

        //Vorbereitete Fragen abrufen
        long lastQuestionRefresh = 0;
        ObservableCollection<QuestionBlock> oc_QuestionBlockSaved;

        public async Task<ObservableCollection<QuestionBlock>> getPreparedQuestionBlocks()
        {
            var timeNow = this.getUnixTimestamp();
            //nach 30 sek. neuer Refresh möglich
            if(timeNow - lastQuestionRefresh > 30)
            {
                string route = "/api/questionBlock";
                oc_QuestionBlockSaved = await sendRequest<ObservableCollection<QuestionBlock>>(route, "GET", null);
                lastQuestionRefresh = this.getUnixTimestamp();
            }
            
            return oc_QuestionBlockSaved;

        }

        //Gruppen abrufen
        public async Task<ObservableCollection<Group>> getGroups()
        {
            var timeNow = this.getUnixTimestamp();

            // only re donwload every 30 seconds
            if ((timeNow - this.LastGroupListRefresh) > 30)
            {
                this.CachedGroupList = await sendRequest<ObservableCollection<Group>>("/api/groups", "GET", null);
                this.LastGroupListRefresh = this.getUnixTimestamp();
            }

            return this.CachedGroupList;
        }

        public async Task<GivenAnswer> sendQuestion(GivenAnswer question)
        {
            var result = await this.sendRequest<GivenAnswer>("/api/givenanswer/start1", "POST", question);

            return result;
        }

        //Gruppe erstellen
        public async void createGroup(string title)
        {
            Group group = new Group() { Title = title, DeviceCount = null, EnterGroupPin = null, Id = null };
            await sendRequest<Group>("/api/groups", "POST", group);
        }


    }
}
